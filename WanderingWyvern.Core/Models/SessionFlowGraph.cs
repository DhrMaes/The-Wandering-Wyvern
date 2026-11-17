namespace DhrMaes.WanderingWyvern.Core.Models;

/// <summary>
/// A node representing a scene or step in a session's decision tree.
/// </summary>
public sealed class SessionFlowNode
{
    public required string Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string NodeType { get; set; } = "standard";
    public List<string> LinkedEntities { get; set; } = [];
    public double X { get; set; }
    public double Y { get; set; }

    public string GetArchetypeIcon() => NodeType.ToLowerInvariant() switch
    {
        "combat" => "⚔️",
        "social" => "🗣️",
        "puzzle" => "🧩",
        "explore" => "🔍",
        "fork" => "🚪",
        "rest" => "🏕️",
        _ => "📖"
    };

    public string GetArchetypeLabel() => NodeType.ToLowerInvariant() switch
    {
        "combat" => "Gevecht",
        "social" => "Sociaal",
        "puzzle" => "Puzzel",
        "explore" => "Verkenning",
        "fork" => "Keuzepunt",
        "rest" => "Rust",
        _ => "Scène"
    };

    public string GetArchetypeColorClass() => $"archetype-{NodeType.ToLowerInvariant()}";

    public SessionFlowNode Clone() => new()
    {
        Id = Id,
        Title = Title,
        Content = Content,
        NodeType = NodeType,
        LinkedEntities = [.. LinkedEntities],
        X = X,
        Y = Y
    };
}

/// <summary>
/// A directed choice or transition between two scenes.
/// </summary>
public sealed class SessionFlowEdge
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public required string SourceNodeId { get; init; }
    public required string TargetNodeId { get; init; }
    public string Label { get; set; } = string.Empty;

    public SessionFlowEdge Clone() => new()
    {
        Id = Id,
        SourceNodeId = SourceNodeId,
        TargetNodeId = TargetNodeId,
        Label = Label
    };
}

/// <summary>
/// A directed graph representing a session's branching scenes and choices.
/// </summary>
public sealed class SessionFlowGraph
{
    public List<SessionFlowNode> Nodes { get; init; } = [];
    public List<SessionFlowEdge> Edges { get; init; } = [];

    public SessionFlowNode? FindNode(string id) =>
        Nodes.FirstOrDefault(n => string.Equals(n.Id, id, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<SessionFlowEdge> GetOutgoingEdges(string nodeId) =>
        Edges.Where(e => string.Equals(e.SourceNodeId, nodeId, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<SessionFlowEdge> GetIncomingEdges(string nodeId) =>
        Edges.Where(e => string.Equals(e.TargetNodeId, nodeId, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<SessionFlowNode> GetReachableNodes(string currentNodeId)
    {
        var targetIds = GetOutgoingEdges(currentNodeId).Select(e => e.TargetNodeId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return Nodes.Where(n => targetIds.Contains(n.Id)).ToList();
    }

    /// <summary>
    /// Finds all node IDs that belong to alternative branches that were not chosen.
    /// Given the sequence of visited node IDs (chosenPath), any node that is neither
    /// on the chosen path nor reachable from the active (latest) node is unchosen.
    /// </summary>
    public HashSet<string> GetUnchosenBranchNodeIds(IEnumerable<string> chosenPathNodeIds)
    {
        var chosenList = chosenPathNodeIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToList();
        if (chosenList.Count == 0 && Nodes.Count > 0)
        {
            chosenList.Add(Nodes[0].Id);
        }

        var chosenSet = new HashSet<string>(chosenList, StringComparer.OrdinalIgnoreCase);
        var activeNodeId = chosenList.LastOrDefault();

        // Nodes reachable from the active node
        var reachableFromActive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(activeNodeId))
        {
            CollectReachable(activeNodeId, reachableFromActive);
        }

        var unchosen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in Nodes)
        {
            if (!chosenSet.Contains(node.Id) && !reachableFromActive.Contains(node.Id))
            {
                unchosen.Add(node.Id);
            }
        }

        return unchosen;
    }

    private void CollectReachable(string nodeId, HashSet<string> visited)
    {
        if (!visited.Add(nodeId))
            return;

        foreach (var edge in GetOutgoingEdges(nodeId))
        {
            CollectReachable(edge.TargetNodeId, visited);
        }
    }

    /// <summary>
    /// Computes a clean hierarchical layout for the nodes on a 2D canvas.
    /// </summary>
    public void CalculateAutoLayout(double nodeWidth = 240, double nodeHeight = 120, double horizontalGap = 80, double verticalGap = 120)
    {
        if (Nodes.Count == 0)
            return;

        // Assign levels using topological rank / BFS
        var levels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var inDegree = Nodes.ToDictionary(n => n.Id, n => GetIncomingEdges(n.Id).Count, StringComparer.OrdinalIgnoreCase);

        var queue = new Queue<string>();
        foreach (var n in Nodes.Where(n => inDegree[n.Id] == 0))
        {
            levels[n.Id] = 0;
            queue.Enqueue(n.Id);
        }

        if (queue.Count == 0 && Nodes.Count > 0)
        {
            levels[Nodes[0].Id] = 0;
            queue.Enqueue(Nodes[0].Id);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var nextLevel = levels[current] + 1;

            foreach (var edge in GetOutgoingEdges(current))
            {
                if (!levels.TryGetValue(edge.TargetNodeId, out var existingLevel) || existingLevel < nextLevel)
                {
                    levels[edge.TargetNodeId] = nextLevel;
                    queue.Enqueue(edge.TargetNodeId);
                }
            }
        }

        // For any unvisited disconnected nodes
        foreach (var node in Nodes)
        {
            if (!levels.ContainsKey(node.Id))
                levels[node.Id] = 0;
        }

        // Group nodes by level
        var nodesByLevel = Nodes.GroupBy(n => levels[n.Id]).OrderBy(g => g.Key).ToList();

        foreach (var group in nodesByLevel)
        {
            var level = group.Key;
            var list = group.ToList();
            var totalWidth = (list.Count * nodeWidth) + ((list.Count - 1) * horizontalGap);
            var startX = Math.Max(40, (800 - totalWidth) / 2);

            for (var i = 0; i < list.Count; i++)
            {
                list[i].X = startX + (i * (nodeWidth + horizontalGap));
                list[i].Y = 60 + (level * (nodeHeight + verticalGap));
            }
        }
    }
}
