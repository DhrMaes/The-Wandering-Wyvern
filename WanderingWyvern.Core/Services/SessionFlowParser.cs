using System.Text;
using System.Text.RegularExpressions;
using DhrMaes.WanderingWyvern.Core.Models;

namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Parses and serializes session Markdown documents containing Mermaid flowcharts and scene headings.
/// </summary>
public sealed class SessionFlowParser
{
    private static readonly Regex MermaidFenceRegex = new(
        @"```(?:mermaid|flowchart)\s*[\r\n]+(?<content>.*?)```",
        RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Matches e.g. S1["1. Het Kruispunt"] or S1[Het Kruispunt] or S1("Title")
    private static readonly Regex NodeDefRegex = new(
        @"^\s*(?<id>[a-zA-Z0-9_\-]+)\s*(?:\[[""']?(?<title>[^""'\]\r\n]+)[""']?\]|\([""']?(?<title>[^""'\)\r\n]+)[""']?\))",
        RegexOptions.Multiline | RegexOptions.Compiled);

    // Matches e.g. S1["1. Het Kruispunt"] -->|Pad links| S2["2A. Het Moeras"] or S1 --> S2
    private static readonly Regex EdgeRegex = new(
        @"(?<source>[a-zA-Z0-9_\-]+)(?:\[[""']?(?<sourceTitle>[^""'\]\r\n]+)[""']?\]|\([""']?(?<sourceTitle>[^""'\)\r\n]+)[""']?\))?\s*(?:-->|-.->|==>)\s*(?:\|(?<label>[^|\r\n]+)\|)?\s*(?<target>[a-zA-Z0-9_\-]+)(?:\[[""']?(?<targetTitle>[^""'\]\r\n]+)[""']?\]|\([""']?(?<targetTitle>[^""'\)\r\n]+)[""']?\))?",
        RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex PositionsCommentRegex = new(
        @"%%\s*positions:\s*(?<positions>[^\r\n]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SceneHeadingRegex = new(
        @"^##\s+(?<title>[^\r\n]+)",
        RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex EntityLinkRegex = new(
        @"\[(?<text>[^\]]+)\]\((?<path>(?:NPCs|Locations|Items|Lore|PCs)/[^\)]+)\)|`(?<path>(?:NPCs|Locations|Items|Lore|PCs)/[^`]+)`",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string DetectArchetype(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return "standard";
        if (title.Contains("⚔") || title.Contains("[combat]", StringComparison.OrdinalIgnoreCase)) return "combat";
        if (title.Contains("🗣") || title.Contains("[social]", StringComparison.OrdinalIgnoreCase)) return "social";
        if (title.Contains("🧩") || title.Contains("[puzzle]", StringComparison.OrdinalIgnoreCase)) return "puzzle";
        if (title.Contains("🔍") || title.Contains("[explore]", StringComparison.OrdinalIgnoreCase)) return "explore";
        if (title.Contains("🚪") || title.Contains("[fork]", StringComparison.OrdinalIgnoreCase)) return "fork";
        if (title.Contains("🏕") || title.Contains("[rest]", StringComparison.OrdinalIgnoreCase)) return "rest";
        return "standard";
    }

    public static List<string> ExtractLinkedEntities(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return [];
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in EntityLinkRegex.Matches(content))
        {
            var path = m.Groups["path"].Value.Trim();
            if (!string.IsNullOrEmpty(path))
            {
                set.Add(path);
            }
        }
        return [.. set];
    }

    public SessionFlowGraph Parse(string markdown)
    {
        var graph = new SessionFlowGraph();
        if (string.IsNullOrWhiteSpace(markdown))
            return graph;

        var mermaidMatch = MermaidFenceRegex.Match(markdown);
        if (mermaidMatch.Success)
        {
            var mermaidContent = mermaidMatch.Groups["content"].Value;
            ParseMermaidContent(mermaidContent, graph);
            AssociateSceneContents(markdown, graph);
        }
        else
        {
            // Fallback for sessions without a Mermaid block: parse ## headings as sequential nodes
            ParseHeadingsAsLinearGraph(markdown, graph);
        }

        // Apply auto-layout if positions are unset
        if (graph.Nodes.Any(n => n.X == 0 && n.Y == 0))
        {
            graph.CalculateAutoLayout();
        }

        return graph;
    }

    private static void ParseMermaidContent(string mermaidContent, SessionFlowGraph graph)
    {
        var nodeMap = new Dictionary<string, SessionFlowNode>(StringComparer.OrdinalIgnoreCase);

        // 1. Parse all node definitions
        foreach (Match match in NodeDefRegex.Matches(mermaidContent))
        {
            var id = match.Groups["id"].Value.Trim();
            if (string.Equals(id, "graph", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "flowchart", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "subgraph", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "end", StringComparison.OrdinalIgnoreCase))
                continue;

            var title = match.Groups["title"].Value.Trim();
            if (!nodeMap.TryGetValue(id, out var node))
            {
                var initialTitle = string.IsNullOrWhiteSpace(title) ? id : title;
                node = new SessionFlowNode
                {
                    Id = id,
                    Title = initialTitle,
                    NodeType = DetectArchetype(initialTitle)
                };
                nodeMap[id] = node;
                graph.Nodes.Add(node);
            }
            else if (!string.IsNullOrWhiteSpace(title) && string.Equals(node.Title, id, StringComparison.OrdinalIgnoreCase))
            {
                node.Title = title;
                node.NodeType = DetectArchetype(title);
            }
        }

        // 2. Parse all edges / transitions
        foreach (Match match in EdgeRegex.Matches(mermaidContent))
        {
            var sourceId = match.Groups["source"].Value.Trim();
            var targetId = match.Groups["target"].Value.Trim();
            var label = match.Groups["label"].Value.Trim();
            var sourceTitle = match.Groups["sourceTitle"].Value.Trim();
            var targetTitle = match.Groups["targetTitle"].Value.Trim();

            if (string.Equals(sourceId, "graph", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(sourceId, "flowchart", StringComparison.OrdinalIgnoreCase))
                continue;

            if (!nodeMap.TryGetValue(sourceId, out var srcNode))
            {
                var srcInitialTitle = string.IsNullOrWhiteSpace(sourceTitle) ? sourceId : sourceTitle;
                srcNode = new SessionFlowNode
                {
                    Id = sourceId,
                    Title = srcInitialTitle,
                    NodeType = DetectArchetype(srcInitialTitle)
                };
                nodeMap[sourceId] = srcNode;
                graph.Nodes.Add(srcNode);
            }
            else if (!string.IsNullOrWhiteSpace(sourceTitle) && string.Equals(srcNode.Title, sourceId, StringComparison.OrdinalIgnoreCase))
            {
                srcNode.Title = sourceTitle;
                srcNode.NodeType = DetectArchetype(sourceTitle);
            }

            if (!nodeMap.TryGetValue(targetId, out var tgtNode))
            {
                var tgtInitialTitle = string.IsNullOrWhiteSpace(targetTitle) ? targetId : targetTitle;
                tgtNode = new SessionFlowNode
                {
                    Id = targetId,
                    Title = tgtInitialTitle,
                    NodeType = DetectArchetype(tgtInitialTitle)
                };
                nodeMap[targetId] = tgtNode;
                graph.Nodes.Add(tgtNode);
            }
            else if (!string.IsNullOrWhiteSpace(targetTitle) && string.Equals(tgtNode.Title, targetId, StringComparison.OrdinalIgnoreCase))
            {
                tgtNode.Title = targetTitle;
                tgtNode.NodeType = DetectArchetype(targetTitle);
            }

            graph.Edges.Add(new SessionFlowEdge
            {
                SourceNodeId = sourceId,
                TargetNodeId = targetId,
                Label = label
            });
        }

        // 3. Parse optional coordinates: %% positions: S1:100,60;S2A:40,240
        var posMatch = PositionsCommentRegex.Match(mermaidContent);
        if (posMatch.Success)
        {
            var pairs = posMatch.Groups["positions"].Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var parts = pair.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length == 2 && nodeMap.TryGetValue(parts[0], out var node))
                {
                    var coords = parts[1].Split(',', StringSplitOptions.TrimEntries);
                    if (coords.Length == 2 &&
                        double.TryParse(coords[0], System.Globalization.CultureInfo.InvariantCulture, out var x) &&
                        double.TryParse(coords[1], System.Globalization.CultureInfo.InvariantCulture, out var y))
                    {
                        node.X = x;
                        node.Y = y;
                    }
                }
            }
        }
    }

    private static void AssociateSceneContents(string markdown, SessionFlowGraph graph)
    {
        // Strip out mermaid block before looking for headings to avoid matching within code
        var cleanedMarkdown = MermaidFenceRegex.Replace(markdown, string.Empty);

        var headingMatches = SceneHeadingRegex.Matches(cleanedMarkdown);
        if (headingMatches.Count == 0)
            return;

        for (var i = 0; i < headingMatches.Count; i++)
        {
            var match = headingMatches[i];
            var headingTitle = match.Groups["title"].Value.Trim();
            var matchedNode = FindMatchingNode(graph, headingTitle);

            if (matchedNode is null)
                continue;

            var startIndex = match.Index + match.Length;

            // Find the index of the next heading that matches a scene node
            var endIndex = cleanedMarkdown.Length;
            for (var j = i + 1; j < headingMatches.Count; j++)
            {
                var nextTitle = headingMatches[j].Groups["title"].Value.Trim();
                if (FindMatchingNode(graph, nextTitle) is not null)
                {
                    endIndex = headingMatches[j].Index;
                    break;
                }
            }

            var content = cleanedMarkdown[startIndex..endIndex].Trim();
            matchedNode.Content = content;
            matchedNode.LinkedEntities = ExtractLinkedEntities(content);
            if (!string.IsNullOrWhiteSpace(headingTitle))
            {
                matchedNode.Title = headingTitle;
                matchedNode.NodeType = DetectArchetype(headingTitle);
            }
        }
    }

    private static SessionFlowNode? FindMatchingNode(SessionFlowGraph graph, string headingTitle)
    {
        if (graph.Nodes.Count == 0 || string.IsNullOrWhiteSpace(headingTitle))
            return null;

        // 1. Exact match on title
        var exact = graph.Nodes.FirstOrDefault(n =>
            string.Equals(n.Title, headingTitle, StringComparison.OrdinalIgnoreCase));
        if (exact is not null) return exact;

        // 2. StartsWith match (either way)
        var startsWith = graph.Nodes.FirstOrDefault(n =>
            headingTitle.StartsWith(n.Title, StringComparison.OrdinalIgnoreCase) ||
            n.Title.StartsWith(headingTitle, StringComparison.OrdinalIgnoreCase));
        if (startsWith is not null) return startsWith;

        // 3. ID match with word boundary (e.g. "S1", "S2A")
        var idMatch = graph.Nodes.FirstOrDefault(n =>
            !string.IsNullOrWhiteSpace(n.Id) &&
            Regex.IsMatch(headingTitle, $@"\b{Regex.Escape(n.Id)}\b", RegexOptions.IgnoreCase));
        if (idMatch is not null) return idMatch;

        // 4. "Scène X" / "Scene X" matching node ID "SX" (e.g. S1 -> Scène 1, S5A -> Scène 5A)
        foreach (var node in graph.Nodes)
        {
            var matchId = Regex.Match(node.Id, @"^S(?<num>\d+[A-Za-z]?)$", RegexOptions.IgnoreCase);
            if (matchId.Success)
            {
                var num = matchId.Groups["num"].Value;
                if (Regex.IsMatch(headingTitle, $@"(?:Scène|Scene|\b)\s*{Regex.Escape(num)}\b", RegexOptions.IgnoreCase))
                {
                    return node;
                }
            }
        }

        // 5. Cleaned title match (without archetype emojis/tags like ⚔️ or [combat])
        var cleanHeading = CleanTitleForComparison(headingTitle);
        if (!string.IsNullOrEmpty(cleanHeading))
        {
            var cleanedMatch = graph.Nodes.FirstOrDefault(n =>
            {
                var cleanNode = CleanTitleForComparison(n.Title);
                return !string.IsNullOrEmpty(cleanNode) && (
                    string.Equals(cleanNode, cleanHeading, StringComparison.OrdinalIgnoreCase) ||
                    cleanHeading.StartsWith(cleanNode, StringComparison.OrdinalIgnoreCase) ||
                    cleanNode.StartsWith(cleanHeading, StringComparison.OrdinalIgnoreCase));
            });
            if (cleanedMatch is not null) return cleanedMatch;
        }

        return null;
    }

    private static string CleanTitleForComparison(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;
        var cleaned = Regex.Replace(title, @"[⚔🗣🧩🔍🚪🏕📖️]+|\[(combat|social|puzzle|explore|fork|rest|standard)\]", string.Empty, RegexOptions.IgnoreCase);
        return cleaned.Trim();
    }

    private static void ParseHeadingsAsLinearGraph(string markdown, SessionFlowGraph graph)
    {
        var headingMatches = SceneHeadingRegex.Matches(markdown);
        SessionFlowNode? prevNode = null;
        var nodeIndex = 1;

        for (var i = 0; i < headingMatches.Count; i++)
        {
            var match = headingMatches[i];
            var headingTitle = match.Groups["title"].Value.Trim();
            var startIndex = match.Index + match.Length;
            var endIndex = (i + 1 < headingMatches.Count) ? headingMatches[i + 1].Index : markdown.Length;
            var content = markdown[startIndex..endIndex].Trim();

            var id = $"S{nodeIndex++}";
            var node = new SessionFlowNode
            {
                Id = id,
                Title = headingTitle,
                Content = content,
                NodeType = DetectArchetype(headingTitle),
                LinkedEntities = ExtractLinkedEntities(content)
            };
            graph.Nodes.Add(node);

            if (prevNode is not null)
            {
                graph.Edges.Add(new SessionFlowEdge
                {
                    SourceNodeId = prevNode.Id,
                    TargetNodeId = node.Id,
                    Label = "Volgende"
                });
            }
            prevNode = node;
        }
    }

    public string Serialize(SessionFlowGraph graph, string? preamble = null)
    {
        var sb = new StringBuilder();

        // 1. Preamble (header / date metadata)
        if (!string.IsNullOrWhiteSpace(preamble))
        {
            sb.AppendLine(preamble.Trim());
            sb.AppendLine();
        }

        // 2. Mermaid block
        sb.AppendLine("```mermaid");
        sb.AppendLine("flowchart TD");

        foreach (var node in graph.Nodes)
        {
            var title = node.Title;
            var icon = node.GetArchetypeIcon();
            if (node.NodeType != "standard" && !title.Contains(icon))
            {
                title = $"{icon} {title}";
            }
            var cleanTitle = title.Replace("\"", "'");
            sb.AppendLine($"    {node.Id}[\"{cleanTitle}\"]");
        }

        foreach (var edge in graph.Edges)
        {
            if (string.IsNullOrWhiteSpace(edge.Label))
            {
                sb.AppendLine($"    {edge.SourceNodeId} --> {edge.TargetNodeId}");
            }
            else
            {
                var cleanLabel = edge.Label.Replace("|", "/");
                sb.AppendLine($"    {edge.SourceNodeId} -->|{cleanLabel}| {edge.TargetNodeId}");
            }
        }

        // Serialize positions comment
        if (graph.Nodes.Any(n => n.X > 0 || n.Y > 0))
        {
            var positions = string.Join(";", graph.Nodes.Select(n =>
                $"{n.Id}:{n.X.ToString("0", System.Globalization.CultureInfo.InvariantCulture)},{n.Y.ToString("0", System.Globalization.CultureInfo.InvariantCulture)}"));
            sb.AppendLine($"    %% positions: {positions}");
        }

        sb.AppendLine("```");
        sb.AppendLine();

        // 3. Scene headings and contents
        foreach (var node in graph.Nodes)
        {
            var title = node.Title;
            var icon = node.GetArchetypeIcon();
            if (node.NodeType != "standard" && !title.Contains(icon))
            {
                title = $"{icon} {title}";
            }
            sb.AppendLine($"## {title}");
            if (!string.IsNullOrWhiteSpace(node.Content))
            {
                sb.AppendLine(node.Content.Trim());
            }
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd() + "\n";
    }

    /// <summary>
    /// Synchronizes outline.md with the latest scenes and referenced entities from the flow graph.
    /// </summary>
    public static string UpdateOutline(string outlineMarkdown, SessionFlowGraph graph)
    {
        if (string.IsNullOrWhiteSpace(outlineMarkdown))
            return outlineMarkdown;

        // 1. Build scenes list
        var scenesBlock = new StringBuilder();
        scenesBlock.AppendLine("## Scènes");
        var sceneIndex = 1;
        foreach (var node in graph.Nodes)
        {
            scenesBlock.AppendLine($"{sceneIndex++}. **{node.Title}:** {node.GetArchetypeLabel()}");
        }

        // 2. Build entities list
        var allEntities = graph.Nodes
            .SelectMany(n => n.LinkedEntities)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(e => e)
            .ToList();

        var entitiesBlock = new StringBuilder();
        entitiesBlock.AppendLine("## Benodigde NPCs/Locaties/Items");
        if (allEntities.Count == 0)
        {
            entitiesBlock.AppendLine("- Geen gekoppelde entiteiten.");
        }
        else
        {
            foreach (var ent in allEntities)
            {
                entitiesBlock.AppendLine($"- [`{ent}`]({ent})");
            }
        }

        var result = outlineMarkdown;

        // Replace or insert ## Scènes
        var scenesRegex = new Regex(@"##\s+Scènes.*?(?=\n##|\z)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (scenesRegex.IsMatch(result))
        {
            result = scenesRegex.Replace(result, scenesBlock.ToString().TrimEnd());
        }
        else
        {
            var neededIndex = result.IndexOf("## Benodigde", StringComparison.OrdinalIgnoreCase);
            if (neededIndex >= 0)
            {
                result = result[..neededIndex].TrimEnd() + "\n\n" + scenesBlock.ToString().TrimEnd() + "\n\n" + result[neededIndex..];
            }
            else
            {
                result = result.TrimEnd() + "\n\n" + scenesBlock.ToString().TrimEnd() + "\n";
            }
        }

        // Replace or append ## Benodigde NPCs/Locaties/Items
        var neededRegex = new Regex(@"##\s+Benodigde\s+NPCs/Locaties/Items.*?(?=\n##|\z)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (neededRegex.IsMatch(result))
        {
            result = neededRegex.Replace(result, entitiesBlock.ToString().TrimEnd());
        }
        else
        {
            result = result.TrimEnd() + "\n\n" + entitiesBlock.ToString().TrimEnd() + "\n";
        }

        return result;
    }

    /// <summary>
    /// Extracts the preamble (e.g. title, metadata before the first Mermaid block, and any introductory content before the first scene).
    /// </summary>
    public static string ExtractPreamble(string markdown, SessionFlowGraph? graph = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var mermaidMatch = MermaidFenceRegex.Match(markdown);
        if (mermaidMatch.Success)
        {
            var preMermaid = markdown[..mermaidMatch.Index].Trim();
            var postMermaidStart = mermaidMatch.Index + mermaidMatch.Length;
            var postMermaid = markdown[postMermaidStart..];

            // Resolve nodes to identify which headings are scenes
            var nodes = graph?.Nodes;
            if (nodes is null || nodes.Count == 0)
            {
                var tempGraph = new SessionFlowGraph();
                ParseMermaidContent(mermaidMatch.Groups["content"].Value, tempGraph);
                nodes = tempGraph.Nodes;
            }

            var tempGraphWrapper = new SessionFlowGraph { Nodes = [.. nodes] };

            // Find where the first scene heading starts in postMermaid
            var headingMatches = SceneHeadingRegex.Matches(postMermaid);
            int firstSceneIndex = -1;

            foreach (Match match in headingMatches)
            {
                var headingTitle = match.Groups["title"].Value.Trim();
                if (FindMatchingNode(tempGraphWrapper, headingTitle) is not null)
                {
                    firstSceneIndex = match.Index;
                    break;
                }
            }

            string postMermaidPreamble = string.Empty;
            if (firstSceneIndex > 0)
            {
                postMermaidPreamble = postMermaid[..firstSceneIndex].Trim();
            }
            else if (firstSceneIndex == -1)
            {
                postMermaidPreamble = headingMatches.Count > 0
                    ? postMermaid[..headingMatches[0].Index].Trim()
                    : postMermaid.Trim();
            }

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(preMermaid))
                parts.Add(preMermaid);
            if (!string.IsNullOrWhiteSpace(postMermaidPreamble))
                parts.Add(postMermaidPreamble);

            return string.Join("\n\n", parts).Trim();
        }

        var fallbackMatch = SceneHeadingRegex.Match(markdown);
        if (fallbackMatch.Success && fallbackMatch.Index > 0)
        {
            return markdown[..fallbackMatch.Index].Trim();
        }

        return string.Empty;
    }
}
