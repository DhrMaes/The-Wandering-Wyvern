using Microsoft.JSInterop;
using DhrMaes.WanderingWyvern.Core.Models;

namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Campaign content store backed by a folder selected in the browser.
/// </summary>
public sealed class BrowserCampaignContentStore : ICampaignContentStore, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public BrowserCampaignContentStore(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "./js/campaignFileSystem.js").AsTask());
    }

    public async Task<IReadOnlyList<string>> ListFilesAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        var files = await module.InvokeAsync<string[]>("listFiles", cancellationToken);
        return files;
    }

    public async Task<IReadOnlyList<CampaignFileMetadata>> GetFileMetadataAsync(
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<CampaignFileMetadata[]>("getFileMetadata", cancellationToken);
    }

    public async Task<string> ReadTextAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<string>("readText", cancellationToken, relativePath);
    }

    public async Task<string> ReadAssetUrlAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<string>("createObjectUrl", cancellationToken, relativePath);
    }

    public async Task ClearObjectUrlsAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("clearObjectUrls", cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("exists", cancellationToken, relativePath);
    }

    public async Task AppendTextAsync(
        string relativePath,
        string content,
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("appendText", cancellationToken, relativePath, content);
    }

    public async Task WriteTextAsync(
        string relativePath,
        string content,
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("writeText", cancellationToken, relativePath, content);
    }

    public async Task<IReadOnlyList<string>> ChooseFolderAsync(
        CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync();
        var files = await module.InvokeAsync<string[]>("chooseFolder", cancellationToken);
        return files;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_moduleTask.IsValueCreated)
            return;

        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("dispose");
        await module.DisposeAsync();
    }

    private Task<IJSObjectReference> GetModuleAsync() => _moduleTask.Value;
}
