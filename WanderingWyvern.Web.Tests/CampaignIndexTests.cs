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

    [TestMethod]
    public void CampaignEntity_CharacterPropertiesAndStandeeDetection_WorkCorrectly()
    {
        var dummyDoc = new CampaignDocument
        {
            RelativePath = "NPCs/robert/robert.md",
            Title = "Robert",
            HasExplicitTitle = true,
            FileName = "robert"
        };

        var logboekDoc = new CampaignDocument
        {
            RelativePath = "NPCs/robert/logboek.md",
            Title = "Logboek",
            HasExplicitTitle = true,
            FileName = "logboek"
        };

        var entity = new CampaignEntity
        {
            Type = CampaignEntityType.Npc,
            Slug = "robert",
            Title = "Robert",
            RelativeFolderPath = "NPCs/robert",
            Documents = new Dictionary<string, CampaignDocument>
            {
                ["robert"] = dummyDoc,
                ["logboek"] = logboekDoc
            },
            Images = ["NPCs/robert/robert.png", "NPCs/robert/robert-standee.png"],
            OtherFiles = ["NPCs/robert/robert-standee.pdf"]
        };

        Assert.IsTrue(entity.IsCharacter);
        Assert.AreEqual("NPCs/robert/robert.png", entity.PortraitImage);
        Assert.AreEqual(1, entity.StandeeImages.Count);
        Assert.AreEqual("NPCs/robert/robert-standee.png", entity.StandeeImages[0]);
        Assert.AreEqual(1, entity.OtherFiles.Count);
        Assert.AreEqual("NPCs/robert/robert-standee.pdf", entity.OtherFiles[0]);
        Assert.AreEqual(2, entity.Documents.Count);
    }
}
