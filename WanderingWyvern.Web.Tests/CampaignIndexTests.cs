using DhrMaes.WanderingWyvern.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
public sealed class CampaignIndexTests
{
    [TestMethod]
    public void CampaignTitle_UsesReadmeTitle_WhenReadmeIsPresent()
    {
        var readme = new CampaignDocument
        {
            RelativePath = "README.md",
            Title = "The Lost Crown of Neverwinter",
            HasExplicitTitle = true,
            FileName = "README"
        };

        var index = new CampaignIndex
        {
            Sessions = [],
            Npcs = [],
            Locations = [],
            Lore = [],
            Pcs = [],
            Items = [],
            Handouts = [],
            DocumentsByPath = new Dictionary<string, CampaignDocument>(StringComparer.OrdinalIgnoreCase)
            {
                ["README.md"] = readme
            }
        };

        Assert.AreEqual("The Lost Crown of Neverwinter", index.CampaignTitle);
        Assert.AreEqual(0, index.TotalEntityCount);
        Assert.IsNotNull(index.Readme);
        Assert.AreEqual("README.md", index.Readme.RelativePath);
    }

    [TestMethod]
    public void CampaignTitle_FallsBackToDefault_WhenReadmeIsNull()
    {
        var index = new CampaignIndex
        {
            Sessions = [],
            Npcs = [],
            Locations = [],
            Lore = [],
            Pcs = [],
            Items = [],
            Handouts = [],
            DocumentsByPath = new Dictionary<string, CampaignDocument>(StringComparer.OrdinalIgnoreCase)
        };

        Assert.AreEqual("The Wandering Wyvern", index.CampaignTitle);
        Assert.AreEqual(0, index.TotalEntityCount);
        Assert.IsNull(index.Readme);
    }

    [TestMethod]
    public void TotalEntityCount_SumsAllEntityCategories()
    {
        var dummyDocument = new CampaignDocument
        {
            RelativePath = "NPCs/alron/alron.md",
            Title = "Alron",
            HasExplicitTitle = true,
            FileName = "alron"
        };

        var entity = new CampaignEntity
        {
            Type = CampaignEntityType.Npc,
            Slug = "alron",
            Title = "Alron",
            RelativeFolderPath = "NPCs/alron",
            Documents = new Dictionary<string, CampaignDocument> { ["alron.md"] = dummyDocument },
            Images = []
        };

        var index = new CampaignIndex
        {
            Sessions = [],
            Npcs = [entity],
            Locations = [],
            Lore = [],
            Pcs = [],
            Items = [],
            Handouts = [],
            DocumentsByPath = new Dictionary<string, CampaignDocument>(StringComparer.OrdinalIgnoreCase)
            {
                ["NPCs/alron/alron.md"] = dummyDocument
            }
        };

        Assert.AreEqual(1, index.TotalEntityCount);
    }
}
