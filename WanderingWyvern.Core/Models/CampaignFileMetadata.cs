namespace DhrMaes.WanderingWyvern.Core.Models;

/// <summary>
/// Lightweight metadata used to detect changes in a browser-selected campaign folder.
/// </summary>
public sealed class CampaignFileMetadata
{
    public required string RelativePath { get; init; }

    public long Size { get; init; }

    public long LastModified { get; init; }
}
