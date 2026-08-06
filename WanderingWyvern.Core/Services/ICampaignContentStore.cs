namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Reads and writes campaign files without exposing where the campaign is stored.
/// </summary>
public interface ICampaignContentStore
{
    Task<IReadOnlyList<string>> ListFilesAsync(CancellationToken cancellationToken = default);

    Task<string> ReadTextAsync(string relativePath, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default);

    Task AppendTextAsync(string relativePath, string content, CancellationToken cancellationToken = default);

    Task WriteTextAsync(string relativePath, string content, CancellationToken cancellationToken = default);
}
