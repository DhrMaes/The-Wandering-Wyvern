namespace DhrMaes.WanderingWyvern.Core.Models;

/// <summary>
/// A logical campaign entity grouping together its Markdown documents and images.
/// </summary>
public sealed class CampaignEntity
{
    public required CampaignEntityType Type { get; init; }

    public required string Slug { get; init; }

    public required string Title { get; init; }

    public required string RelativeFolderPath { get; init; }

    public required IReadOnlyDictionary<string, CampaignDocument> Documents { get; init; }

    public required IReadOnlyList<string> Images { get; init; }

    public IReadOnlyList<string> Maps { get; init; } = [];

    public string? IconImage { get; init; }

    public CampaignDocument? PrimaryDocument
    {
        get
        {
            if (Documents.Count == 0)
                return null;

            if (Documents.TryGetValue(Slug, out var matching))
                return matching;

            if (Documents.TryGetValue("outline", out var outline))
                return outline;

            return Documents
                .OrderBy(document => document.Key, StringComparer.OrdinalIgnoreCase)
                .Select(document => document.Value)
                .First();
        }
    }

    public string? CardImage => IconImage ?? Images.FirstOrDefault() ?? Maps.FirstOrDefault();
}
