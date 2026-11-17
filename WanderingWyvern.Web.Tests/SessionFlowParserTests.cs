using DhrMaes.WanderingWyvern.Core.Models;
using DhrMaes.WanderingWyvern.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
public sealed class SessionFlowParserTests
{
    private readonly SessionFlowParser _parser = new();

    [TestMethod]
    public void Parse_WithMermaidBlock_ExtractsNodesEdgesAndSceneContent()
    {
        var markdown = """
            # Sessie 1 — Het Verloren Pad
            **Datum:** 2026-11-05

            ```mermaid
            flowchart TD
                S1["1. Het Kruispunt"] -->|Pad links| S2A["2A. Het Moeras"]
                S1 -->|Pad rechts| S2B["2B. De Rivier"]
                S2A -->|Aanvallen| S3["3. Het Kamp"]
            ```

            ## 1. Het Kruispunt
            > "De weg splitst zich in twee modderige sporen."
            Survival check DC 12 om sporen te zien.

            ## 2A. Het Moeras
            > "Mist stijgt op uit het zwarte water."
            Goblins loeren in het riet.

            ## 2B. De Rivier
            > "Een wilde stroom verspert de weg."

            ## 3. Het Kamp
            > "Rook kringelt op uit het tentenkamp."
            """;

        var graph = _parser.Parse(markdown);

        Assert.AreEqual(4, graph.Nodes.Count);
        Assert.AreEqual(3, graph.Edges.Count);

        var s1 = graph.FindNode("S1");
        Assert.IsNotNull(s1);
        Assert.AreEqual("1. Het Kruispunt", s1.Title);
        StringAssert.Contains(s1.Content, "Survival check DC 12");

        var s2a = graph.FindNode("S2A");
        Assert.IsNotNull(s2a);
        Assert.AreEqual("2A. Het Moeras", s2a.Title);
        StringAssert.Contains(s2a.Content, "Goblins loeren in het riet");

        var s1Outgoing = graph.GetOutgoingEdges("S1");
        Assert.AreEqual(2, s1Outgoing.Count);
        Assert.IsTrue(s1Outgoing.Any(e => e.TargetNodeId == "S2A" && e.Label == "Pad links"));
        Assert.IsTrue(s1Outgoing.Any(e => e.TargetNodeId == "S2B" && e.Label == "Pad rechts"));
    }

    [TestMethod]
    public void Parse_EchoesOfTheUnderdark_ExtractsSceneContent()
    {
        var path = @"C:\Users\ArneMA\OneDrive - Skyline Communications NV\Personal\Dnd\Echoes of the Underdark\Sessions\session-01-echoes-of-the-underdark\script.md";
        Assert.IsTrue(System.IO.File.Exists(path), $"File does not exist: {path}");

        var markdown = System.IO.File.ReadAllText(path);
        var graph = _parser.Parse(markdown);

        var s1 = graph.FindNode("S1");
        Assert.IsNotNull(s1);
        Assert.IsFalse(string.IsNullOrWhiteSpace(s1.Content), "S1.Content should not be empty!");
        StringAssert.Contains(s1.Content, "De stank van verbrand vlees");

        var preamble = SessionFlowParser.ExtractPreamble(markdown, graph);
        StringAssert.Contains(preamble, "De Mutatie-Vloek");

        var index = new CampaignIndex
        {
            Sessions = [],
            Npcs = [],
            Locations = [],
            Lore = [],
            Pcs = [],
            Items = [],
            Handouts = [],
            DocumentsByPath = new Dictionary<string, CampaignDocument>()
        };
        var renderer = new CampaignMarkdownRenderer(new BrowserCampaignContentStore(null!));
        var preambleHtml = renderer.RenderMarkdownSnippetToHtml(preamble, "Sessions/session-01-echoes-of-the-underdark/script.md", index);
        var contentHtml = renderer.RenderMarkdownSnippetToHtml(s1.Content, "Sessions/session-01-echoes-of-the-underdark/script.md", index);

        Assert.IsFalse(string.IsNullOrWhiteSpace(preambleHtml));
        Assert.IsFalse(string.IsNullOrWhiteSpace(contentHtml));
        StringAssert.Contains(contentHtml, "De stank van verbrand vlees");
    }

    [TestMethod]
    public void Parse_WithoutMermaid_ExtractsLinearGraphFromHeadings()
    {
        var markdown = """
            # Sessie 2 — Lineaire Sessie

            ## Eerste Scène
            Beschrijving van scène 1.

            ## Tweede Scène
            Beschrijving van scène 2.
            """;

        var graph = _parser.Parse(markdown);

        Assert.AreEqual(2, graph.Nodes.Count);
        Assert.AreEqual(1, graph.Edges.Count);
        Assert.AreEqual("Eerste Scène", graph.Nodes[0].Title);
        Assert.AreEqual("Tweede Scène", graph.Nodes[1].Title);
    }

    [TestMethod]
    public void GetUnchosenBranchNodeIds_CalculatesPrunedBranchesCorrectly()
    {
        var graph = new SessionFlowGraph();
        graph.Nodes.Add(new SessionFlowNode { Id = "S1", Title = "Start" });
        graph.Nodes.Add(new SessionFlowNode { Id = "S2A", Title = "Links" });
        graph.Nodes.Add(new SessionFlowNode { Id = "S2B", Title = "Rechts" });
        graph.Nodes.Add(new SessionFlowNode { Id = "S3A", Title = "Links Vervolg" });
        graph.Nodes.Add(new SessionFlowNode { Id = "S3B", Title = "Rechts Vervolg" });

        graph.Edges.Add(new SessionFlowEdge { SourceNodeId = "S1", TargetNodeId = "S2A", Label = "Links" });
        graph.Edges.Add(new SessionFlowEdge { SourceNodeId = "S1", TargetNodeId = "S2B", Label = "Rechts" });
        graph.Edges.Add(new SessionFlowEdge { SourceNodeId = "S2A", TargetNodeId = "S3A", Label = "Verder" });
        graph.Edges.Add(new SessionFlowEdge { SourceNodeId = "S2B", TargetNodeId = "S3B", Label = "Verder" });

        // Player chose S1 -> S2A
        var unchosen = graph.GetUnchosenBranchNodeIds(["S1", "S2A"]);

        Assert.IsTrue(unchosen.Contains("S2B"), "S2B should be unchosen");
        Assert.IsTrue(unchosen.Contains("S3B"), "S3B (descendant of S2B) should be unchosen");
        Assert.IsFalse(unchosen.Contains("S1"), "S1 was visited");
        Assert.IsFalse(unchosen.Contains("S2A"), "S2A was visited");
        Assert.IsFalse(unchosen.Contains("S3A"), "S3A is a valid reachable future node");
    }

    [TestMethod]
    public void Serialize_RoundtripsGraphCleanly()
    {
        var graph = new SessionFlowGraph();
        graph.Nodes.Add(new SessionFlowNode
        {
            Id = "S1",
            Title = "Kruispunt",
            Content = "> \"Waar gaan we heen?\"",
            X = 100,
            Y = 50
        });
        graph.Nodes.Add(new SessionFlowNode
        {
            Id = "S2",
            Title = "Het Moeras",
            Content = "> \"Zompig land.\"",
            X = 100,
            Y = 200
        });
        graph.Edges.Add(new SessionFlowEdge
        {
            SourceNodeId = "S1",
            TargetNodeId = "S2",
            Label = "Pad links"
        });

        var serialized = _parser.Serialize(graph, "# Sessie Titel");
        StringAssert.Contains(serialized, "```mermaid");
        StringAssert.Contains(serialized, "flowchart TD");
        StringAssert.Contains(serialized, "S1[\"Kruispunt\"]");
        StringAssert.Contains(serialized, "S1 -->|Pad links| S2");
        StringAssert.Contains(serialized, "## Kruispunt");
        StringAssert.Contains(serialized, "> \"Waar gaan we heen?\"");

        var parsedAgain = _parser.Parse(serialized);
        Assert.AreEqual(2, parsedAgain.Nodes.Count);
        Assert.AreEqual(1, parsedAgain.Edges.Count);
        Assert.AreEqual("Kruispunt", parsedAgain.Nodes[0].Title);
        Assert.AreEqual("Pad links", parsedAgain.Edges[0].Label);
    }

    [TestMethod]
    public void Templates_AllDefinitions_ProduceValidParsableGraphs()
    {
        Assert.AreEqual(5, SessionFlowTemplates.All.Count);

        foreach (var tmpl in SessionFlowTemplates.All)
        {
            var graph = _parser.Parse(tmpl.DefaultScriptMarkdown);
            Assert.IsTrue(graph.Nodes.Count >= 1, $"Template {tmpl.Id} should have nodes");
            if (tmpl.Id != SessionFlowTemplates.BlankCanvasId)
            {
                Assert.IsTrue(graph.Edges.Count >= 1, $"Template {tmpl.Id} should have edges");
            }

            // Verify archetypes are parsed
            Assert.IsTrue(graph.Nodes.Any(n => !string.IsNullOrEmpty(n.NodeType)), $"Template {tmpl.Id} nodes should have NodeType set");
        }
    }

    [TestMethod]
    public void DetectArchetype_And_ExtractLinkedEntities_WorksCorrectly()
    {
        Assert.AreEqual("combat", SessionFlowParser.DetectArchetype("⚔️ Scène 1 — Gevecht"));
        Assert.AreEqual("social", SessionFlowParser.DetectArchetype("🗣️ Scène 2 — Gesprek"));
        Assert.AreEqual("puzzle", SessionFlowParser.DetectArchetype("🧩 Scène 3 — Puzzel"));
        Assert.AreEqual("explore", SessionFlowParser.DetectArchetype("🔍 Scène 4 — Verkenning"));
        Assert.AreEqual("fork", SessionFlowParser.DetectArchetype("🚪 Scène 5 — Keuze"));
        Assert.AreEqual("rest", SessionFlowParser.DetectArchetype("🏕️ Scène 6 — Rust"));
        Assert.AreEqual("standard", SessionFlowParser.DetectArchetype("Scène 7 — Zonder icoon"));

        var content = """
            De groep praat met [Robert](NPCs/robert/robert.md) en reist naar `Locations/het-moeras/het-moeras.md`.
            Later ontdekken ze [Zwaard van de Koning](Items/zwaard/zwaard.md).
            """;

        var entities = SessionFlowParser.ExtractLinkedEntities(content);
        Assert.AreEqual(3, entities.Count);
        Assert.IsTrue(entities.Contains("NPCs/robert/robert.md"));
        Assert.IsTrue(entities.Contains("Locations/het-moeras/het-moeras.md"));
        Assert.IsTrue(entities.Contains("Items/zwaard/zwaard.md"));
    }

    [TestMethod]
    public void UpdateOutline_SynchronizesScenesAndEntities_Accurately()
    {
        var outline = """
            # Sessie 1 — Het Kruispunt
            **Datum:** 2026-11-09

            ## Doel van de sessie
            Het kruispunt verkennen.

            ## Benodigde NPCs/Locaties/Items
            - Geen gekoppelde entiteiten.
            """;

        var graph = new SessionFlowGraph();
        graph.Nodes.Add(new SessionFlowNode
        {
            Id = "S1",
            Title = "Het Kruispunt",
            NodeType = "fork",
            LinkedEntities = ["NPCs/robert/robert.md"]
        });
        graph.Nodes.Add(new SessionFlowNode
        {
            Id = "S2",
            Title = "Het Moeras",
            NodeType = "explore",
            LinkedEntities = ["Locations/het-moeras/het-moeras.md", "NPCs/robert/robert.md"]
        });

        var updated = SessionFlowParser.UpdateOutline(outline, graph);

        StringAssert.Contains(updated, "## Scènes");
        StringAssert.Contains(updated, "1. **Het Kruispunt:** Keuzepunt");
        StringAssert.Contains(updated, "2. **Het Moeras:** Verkenning");

        StringAssert.Contains(updated, "## Benodigde NPCs/Locaties/Items");
        StringAssert.Contains(updated, "[`NPCs/robert/robert.md`](NPCs/robert/robert.md)");
        StringAssert.Contains(updated, "[`Locations/het-moeras/het-moeras.md`](Locations/het-moeras/het-moeras.md)");
    }

    [TestMethod]
    public void Parse_PartySplitAndReunionFlow_CorrectlyResolvesParallelBranchesAndConvergence()
    {
        var markdown = """
            # Sessie 1 — Het Gesplitste Pad

            ```mermaid
            flowchart TD
                S1["1. De Splitsing"] -->|Groep A volgt de rivier| S2A["2A. De Rivier"]
                S1 -->|Groep B beklimt de bergkam| S2B["2B. De Bergkam"]
                S2A -->|Aankomst bij kasteel| S3["3. Kasteelpoort"]
                S2B -->|Afdalen naar kasteel| S3
            ```

            ## 1. De Splitsing
            > "De groep besluit zich op te splitsen om twee kanten te bespioneren."

            ## 2A. De Rivier
            > "Langs het kabbelende water..."

            ## 2B. De Bergkam
            > "De gure wind giert over de toppen..."

            ## 3. Kasteelpoort
            > "Beide groepen zien elkaar weer bij de massieve poort."
            """;

        var graph = _parser.Parse(markdown);

        Assert.AreEqual(4, graph.Nodes.Count);
        Assert.AreEqual(4, graph.Edges.Count);

        // Verify split from S1
        var forkEdges = graph.GetOutgoingEdges("S1");
        Assert.AreEqual(2, forkEdges.Count);
        CollectionAssert.AreEquivalent(new[] { "S2A", "S2B" }, forkEdges.Select(e => e.TargetNodeId).ToList());

        // Verify convergence at S3
        var s3Incoming = graph.Edges.Where(e => string.Equals(e.TargetNodeId, "S3", StringComparison.OrdinalIgnoreCase)).ToList();
        Assert.AreEqual(2, s3Incoming.Count);
        CollectionAssert.AreEquivalent(new[] { "S2A", "S2B" }, s3Incoming.Select(e => e.SourceNodeId).ToList());
    }

    [TestMethod]
    public void Parse_WithSubheadingsAndIntroRules_PreservesSceneContentAndPreamble()
    {
        var markdown = """
            # Sessie 01 — Voorleesscript voor de DM

            **Datum:** 2026-11-16  
            *Zie het bijbehorende regie-overzicht in [outline.md](outline.md).*

            ```mermaid
            flowchart TD
                S1["Scène 1 — Het Giftige Lab & De Tikkende Klok"] -->|Volg de brede noordelijke kloof| S2["Scène 2 — De Schreeuwende Kloof"]
                S1 -->|Kies de smalle Faerzress-spleet| S3["Scène 3 — De Faerzress Schikking"]
                S2 --> S4["Scène 4 — De Myconid Grove & Het Oordeel"]
                S3 --> S4
            ```

            ---

            ## De Mutatie-Vloek & Huisregels voor de Sessie

            Leg de spelers vóór de start van de sessie de twee actieve mechanismen uit:

            ### 1. De Klok (48 Uur tot Definitieve Mutatie)
            De groep heeft nog **42 uur** over.

            ### 2. Mutatie Stadia (Fase 1: Direct Actief)
            - **[Perseus](PCs/perseus/perseus.md) — *Bioluminescente Pustels*:** Op je hals en jukbeenderen.

            ---

            ## Scène 1 — Het Giftige Lab & De Tikkende Klok

            ### Voorleestekst (Start van de Sessie)
            > "De stank van verbrand vlees en chemische dampen hangt zwaar in het laboratorium."

            ### Tips & Telegrafie voor de Spelers (Hoe stuur je de opties aan?)
            Als de spelers niet meteen weten wat te doen:
            1. **De Tijdnood:** Wijs op de donkerpaarse aderen.

            ### Mogelijke Checks & DC Gradiënten

            #### 1. Het Laboratorium & De Aantekeningen Doorzoeken
            *Intelligence (Investigation) check op de werktafels:*
            - **DC 8:** Je vindt tussen gebroken kolven [Het Notitieboek van de Professor](Items/professors-notitieboek/professors-notitieboek.md).
            - **DC 12:** Je ontdekt een ruwe anatomische schets.

            ---

            ## Scène 2 — De Schreeuwende Kloof

            ### Voorleestekst (De Koude Tocht)
            > "Een ijzige tocht giert door de diepe rotsspleet."
            """;

        var graph = _parser.Parse(markdown);

        // Assert nodes
        Assert.AreEqual(4, graph.Nodes.Count);
        var s1 = graph.FindNode("S1");
        Assert.IsNotNull(s1);
        Assert.AreEqual("Scène 1 — Het Giftige Lab & De Tikkende Klok", s1.Title);

        // S1 content must include subheadings, blockquotes, and checks (NOT empty!)
        Assert.IsFalse(string.IsNullOrWhiteSpace(s1.Content));
        StringAssert.Contains(s1.Content, "Voorleestekst (Start van de Sessie)");
        StringAssert.Contains(s1.Content, "De stank van verbrand vlees");
        StringAssert.Contains(s1.Content, "Tips & Telegrafie voor de Spelers");
        StringAssert.Contains(s1.Content, "Mogelijke Checks & DC Gradiënten");
        StringAssert.Contains(s1.Content, "Het Laboratorium & De Aantekeningen Doorzoeken");
        StringAssert.Contains(s1.Content, "Je vindt tussen gebroken kolven");
        CollectionAssert.Contains(s1.LinkedEntities, "Items/professors-notitieboek/professors-notitieboek.md");

        // S2 content must contain its text
        var s2 = graph.FindNode("S2");
        Assert.IsNotNull(s2);
        StringAssert.Contains(s2.Content, "Een ijzige tocht giert door de diepe rotsspleet");

        // Preamble must contain title, date, AND De Mutatie-Vloek & Huisregels section
        var preamble = SessionFlowParser.ExtractPreamble(markdown, graph);
        StringAssert.Contains(preamble, "Sessie 01 — Voorleesscript voor de DM");
        StringAssert.Contains(preamble, "2026-11-16");
        StringAssert.Contains(preamble, "De Mutatie-Vloek & Huisregels voor de Sessie");
        StringAssert.Contains(preamble, "De Klok (48 Uur tot Definitieve Mutatie)");
        StringAssert.Contains(preamble, "PCs/perseus/perseus.md");

        // Without passing graph, ExtractPreamble should still work via Mermaid node extraction
        var preambleWithoutGraph = SessionFlowParser.ExtractPreamble(markdown);
        StringAssert.Contains(preambleWithoutGraph, "De Mutatie-Vloek & Huisregels voor de Sessie");
        StringAssert.Contains(preambleWithoutGraph, "De Klok (48 Uur tot Definitieve Mutatie)");
    }
}
