using DhrMaes.WanderingWyvern.Core.Models;
using DhrMaes.WanderingWyvern.Core.Services;

namespace DhrMaes.WanderingWyvern.Web.Client;

/// <summary>
/// Browser-local campaign state shared by WebAssembly components.
/// </summary>
public sealed class CampaignClientState : IAsyncDisposable
{
    private readonly BrowserCampaignContentStore _contentStore;
    private readonly CampaignIndexBuilder _indexBuilder = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private CancellationTokenSource? _monitorCancellation;
    private Task? _monitorTask;
    private IReadOnlyList<CampaignFileMetadata> _fileMetadata = [];

    public CampaignClientState(BrowserCampaignContentStore contentStore)
    {
        _contentStore = contentStore;
    }

    public CampaignIndex? Current { get; private set; }

    public bool HasSelectedFolder => Current is not null;

    public int FileCount { get; private set; }

    public event Action? Changed;

    public async Task ChooseFolderAsync(CancellationToken cancellationToken = default)
    {
        await StopMonitorAsync();
        var files = await _contentStore.ChooseFolderAsync(cancellationToken);
        Current = await _indexBuilder.BuildAsync(_contentStore, cancellationToken);
        FileCount = files.Count;
        _fileMetadata = await ReadFileMetadataAsync(cancellationToken);
        StartMonitor();
        Changed?.Invoke();
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (!HasSelectedFolder)
            return;

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            var metadata = await ReadFileMetadataAsync(cancellationToken);
            await _contentStore.ClearObjectUrlsAsync(cancellationToken);
            Current = await _indexBuilder.BuildAsync(_contentStore, cancellationToken);
            FileCount = metadata.Count;
            _fileMetadata = metadata;
            Changed?.Invoke();
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public Task<string> ReadAssetUrlAsync(
        string relativePath,
        CancellationToken cancellationToken = default) =>
        _contentStore.ReadAssetUrlAsync(relativePath, cancellationToken);

    public async Task<string?> ReadNotesAsync(
        string sessionSlug,
        CancellationToken cancellationToken = default)
    {
        var path = $"Sessions/{sessionSlug}/live-notes.md";
        return await _contentStore.ExistsAsync(path, cancellationToken)
            ? await _contentStore.ReadTextAsync(path, cancellationToken)
            : null;
    }

    public async Task AppendNoteAsync(
        string sessionSlug,
        string note,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(note))
            return;

        var path = $"Sessions/{sessionSlug}/live-notes.md";
        if (!await _contentStore.ExistsAsync(path, cancellationToken))
            await _contentStore.WriteTextAsync(path, "# Live notities\n\n", cancellationToken);

        var timestamp = DateTime.Now.ToString("HH:mm");
        await _contentStore.AppendTextAsync(path, $"- **{timestamp}** — {note.Trim()}\n", cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await StopMonitorAsync();
        _refreshLock.Dispose();
        await _contentStore.DisposeAsync();
    }

    private async Task<IReadOnlyList<CampaignFileMetadata>> ReadFileMetadataAsync(
        CancellationToken cancellationToken) =>
        (await _contentStore.GetFileMetadataAsync(cancellationToken))
        .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private void StartMonitor()
    {
        _monitorCancellation = new CancellationTokenSource();
        _monitorTask = MonitorAsync(_monitorCancellation.Token);
    }

    private async Task StopMonitorAsync()
    {
        if (_monitorCancellation is null)
            return;

        _monitorCancellation.Cancel();
        if (_monitorTask is not null)
        {
            try
            {
                await _monitorTask;
            }
            catch (OperationCanceledException) when (_monitorCancellation.IsCancellationRequested)
            {
            }
        }

        _monitorCancellation.Dispose();
        _monitorCancellation = null;
        _monitorTask = null;
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var metadata = await ReadFileMetadataAsync(cancellationToken);
            if (metadata.SequenceEqual(_fileMetadata, CampaignFileMetadataComparer.Instance))
                continue;

            await RefreshAsync(cancellationToken);
        }
    }

    private sealed class CampaignFileMetadataComparer : IEqualityComparer<CampaignFileMetadata>
    {
        public static readonly CampaignFileMetadataComparer Instance = new();

        public bool Equals(CampaignFileMetadata? left, CampaignFileMetadata? right) =>
            left?.RelativePath.Equals(right?.RelativePath, StringComparison.OrdinalIgnoreCase) == true
            && left.Size == right.Size
            && left.LastModified == right.LastModified;

        public int GetHashCode(CampaignFileMetadata obj) =>
            HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.RelativePath),
                obj.Size,
                obj.LastModified);
    }

}
