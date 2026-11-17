using DhrMaes.WanderingWyvern.Core.Models;
using DhrMaes.WanderingWyvern.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
public sealed class EntityMetadataExtractorTests
{
    [TestMethod]
    public void Extract_NpcWithCompositeBasisAndMetadata_ReturnsCorrectPills()
    {
        var markdown = """
            # Robert

            Basis: First Class (Rijk/Machtig), Boekhandelaar
            Ras: Mens
            Factie: De Harpers
            **Type NPC:** Named

            ## Uiterlijk
            Robert is een verzorgde man...
            """;

        var items = EntityMetadataExtractor.Extract(markdown, CampaignEntityType.Npc);

        Assert.AreEqual(5, items.Count);
        Assert.IsTrue(items.Any(i => i.Label == "Klasse" && i.Value == "First Class (Rijk/Machtig)"));
        Assert.IsTrue(items.Any(i => i.Label == "Beroep" && i.Value == "Boekhandelaar"));
        Assert.IsTrue(items.Any(i => i.Label == "Ras" && i.Value == "Mens"));
        Assert.IsTrue(items.Any(i => i.Label == "Factie" && i.Value == "De Harpers"));
        Assert.IsTrue(items.Any(i => i.Label == "Type" && i.Value == "Named"));
    }

    [TestMethod]
    public void Extract_PcWithPlayerAndClassAndLevel_ReturnsCorrectPills()
    {
        var markdown = """
            # Prinses Bibeth

            Speler: Jan
            Race: Human
            Class: Rogue (Swashbuckler)
            Level: 4

            ## Uiterlijk
            Zij draagt een elegante cape...
            """;

        var items = EntityMetadataExtractor.Extract(markdown, CampaignEntityType.Pc);

        Assert.AreEqual(4, items.Count);
        Assert.IsTrue(items.Any(i => i.Label == "Speler" && i.Value == "Jan"));
        Assert.IsTrue(items.Any(i => i.Label == "Ras" && i.Value == "Human"));
        Assert.IsTrue(items.Any(i => i.Label == "Class" && i.Value == "Rogue (Swashbuckler)"));
        Assert.IsTrue(items.Any(i => i.Label == "Level" && i.Value == "4"));
    }

    [TestMethod]
    public void Extract_EmptyOrNoMetadata_ReturnsEmptyList()
    {
        var markdown = """
            # Een Oud Zwaard

            Dit is gewoon een oud zwaard zonder metadata.
            """;

        var items = EntityMetadataExtractor.Extract(markdown, CampaignEntityType.Item);

        Assert.AreEqual(0, items.Count);
    }
}
