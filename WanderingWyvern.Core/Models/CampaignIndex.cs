namespace DhrMaes.WanderingWyvern.Core.Models;

/// <summary>
/// A fully scanned snapshot of the campaign content repository.
/// </summary>
public sealed class CampaignIndex
{
    public required IReadOnlyList<CampaignEntity> Sessions { get; init; }

    public required IReadOnlyList<CampaignEntity> Npcs { get; init; }

    public required IReadOnlyList<CampaignEntity> Locations { get; init; }

    public required IReadOnlyList<CampaignEntity> Lore { get; init; }

    public required IReadOnlyList<CampaignEntity> Pcs { get; init; }

    public required IReadOnlyList<CampaignEntity> Items { get; init; }

    public required IReadOnlyList<CampaignEntity> Handouts { get; init; }

    public required IReadOnlyDictionary<string, CampaignDocument> DocumentsByPath { get; init; }

    public CampaignDocument? Readme => FindDocument("README.md");

    public string CampaignTitle => Readme?.Title ?? "The Wandering Wyvern";

    public int TotalEntityCount => Sessions.Count + Npcs.Count + Locations.Count + Lore.Count + Pcs.Count + Items.Count + Handouts.Count;

    public CampaignEntity? FindSession(string slug) =>
        Sessions.FirstOrDefault(s => string.Equals(s.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public CampaignDocument? FindDocument(string normalizedRelativePath) =>
        DocumentsByPath.GetValueOrDefault(normalizedRelativePath);

    public CampaignDocument? FindDocumentReference(string normalizedPath)
    {
        var document = FindDocument(normalizedPath);
        if (document is not null)
            return document;

        return AllEntities()
            .FirstOrDefault(entity =>
                string.Equals(entity.RelativeFolderPath, normalizedPath, StringComparison.OrdinalIgnoreCase))
            ?.PrimaryDocument;
    }

    public CampaignEntity? FindEntityForDocument(string relativePath) =>
        AllEntities().FirstOrDefault(entity =>
            entity.Documents.Values.Any(document =>
                string.Equals(document.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase)));

    private IEnumerable<CampaignEntity> AllEntities() =>
        Sessions
            .Concat(Npcs)
            .Concat(Locations)
            .Concat(Lore)
            .Concat(Pcs)
            .Concat(Items)
            .Concat(Handouts);
}
