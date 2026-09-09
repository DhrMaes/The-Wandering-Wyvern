using DhrMaes.WanderingWyvern.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
public sealed class CampaignAiTemplatesTests
{
    private sealed class InMemoryContentStore : ICampaignContentStore
    {
        public Dictionary<string, string> Storage { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Task<IReadOnlyList<string>> ListFilesAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<string> list = Storage.Keys.ToList();
            return Task.FromResult(list);
        }

        public Task<string> ReadTextAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            if (Storage.TryGetValue(relativePath, out var text))
            {
                return Task.FromResult(text);
            }
            throw new FileNotFoundException(relativePath);
        }

        public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Storage.ContainsKey(relativePath));
        }

        public Task AppendTextAsync(string relativePath, string content, CancellationToken cancellationToken = default)
        {
            Storage[relativePath] = Storage.GetValueOrDefault(relativePath, "") + content;
            return Task.CompletedTask;
        }

        public Task WriteTextAsync(string relativePath, string content, CancellationToken cancellationToken = default)
        {
            Storage[relativePath] = content;
            return Task.CompletedTask;
        }
    }

    [TestMethod]
    public void AllTemplates_MatchOnDiskFiles()
    {
        // Locate campaign-template directory from repository
        var solutionDir = FindSolutionDirectory();
        var templateDir = Path.Combine(solutionDir, "WanderingWyvern.Web.Client", "wwwroot", "campaign-template");
        Assert.IsTrue(Directory.Exists(templateDir), $"Template directory not found: {templateDir}");

        var diskFiles = Directory.GetFiles(templateDir, "*", SearchOption.AllDirectories);
        Assert.AreEqual(diskFiles.Length, CampaignAiTemplates.Files.Count, "Count of files in template should match CampaignAiTemplates");

        foreach (var file in diskFiles)
        {
            var relativePath = Path.GetRelativePath(templateDir, file).Replace('\\', '/');
            Assert.IsTrue(CampaignAiTemplates.Files.ContainsKey(relativePath), $"Missing template key: {relativePath}");
            var diskContent = File.ReadAllText(file);
            Assert.AreEqual(diskContent, CampaignAiTemplates.Files[relativePath], $"Content mismatch for template: {relativePath}");
        }
    }

    [TestMethod]
    public async Task SyncAsync_WritesAllFiles_WhenContentStoreIsEmpty()
    {
        var store = new InMemoryContentStore();
        var count = await CampaignAiTemplates.SyncAsync(store);

        Assert.AreEqual(CampaignAiTemplates.Files.Count, count);
        Assert.AreEqual(CampaignAiTemplates.Files.Count, store.Storage.Count);

        foreach (var (path, content) in CampaignAiTemplates.Files)
        {
            Assert.IsTrue(store.Storage.ContainsKey(path), $"Path {path} should be written");
            Assert.AreEqual(content, store.Storage[path]);
        }
    }

    [TestMethod]
    public async Task SyncAsync_DoesNotRewrite_WhenFilesMatch()
    {
        var store = new InMemoryContentStore();
        await CampaignAiTemplates.SyncAsync(store);

        // Second sync should find all files identical and write 0 files
        var secondCount = await CampaignAiTemplates.SyncAsync(store);
        Assert.AreEqual(0, secondCount);
    }

    [TestMethod]
    public async Task SyncAsync_Overwrites_WhenFileDiffers()
    {
        var store = new InMemoryContentStore();
        await CampaignAiTemplates.SyncAsync(store);

        // Modify one file
        var key = "AGENTS.md";
        store.Storage[key] = "Old content";

        var updateCount = await CampaignAiTemplates.SyncAsync(store);
        Assert.AreEqual(1, updateCount);
        Assert.AreEqual(CampaignAiTemplates.Files[key], store.Storage[key]);
    }

    private static string FindSolutionDirectory()
    {
        var current = AppContext.BaseDirectory;
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current, "WanderingWyvern.slnx")) || Directory.Exists(Path.Combine(current, ".git")))
            {
                return current;
            }
            current = Directory.GetParent(current)?.FullName;
        }
        throw new InvalidOperationException("Could not find repository root directory");
    }
}
