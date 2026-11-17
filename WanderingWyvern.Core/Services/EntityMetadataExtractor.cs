using DhrMaes.WanderingWyvern.Core.Models;

namespace DhrMaes.WanderingWyvern.Core.Services;

public sealed record EntityMetaItem(string Icon, string Label, string Value);

/// <summary>
/// Extracts structured metadata tags (Class, Job, Race, Player, Level, etc.) from entity Markdown sheets.
/// </summary>
public static class EntityMetadataExtractor
{
    public static IReadOnlyList<EntityMetaItem> Extract(string? markdown, CampaignEntityType entityType)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return [];

        var items = new List<EntityMetaItem>();
        var lines = markdown.Split(["\r\n", "\n"], StringSplitOptions.None);

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Stop when we reach the first major content heading after the main title
            if (line.StartsWith("## ", StringComparison.Ordinal) || line.StartsWith("### ", StringComparison.Ordinal))
                break;

            // Skip main # Title
            if (line.StartsWith("# ", StringComparison.Ordinal))
                continue;

            var colonIndex = line.IndexOf(':');
            if (colonIndex <= 0)
                continue;

            var rawKey = line[..colonIndex].Trim().TrimStart('-', '*', ' ').TrimEnd('*', ' ').Trim();
            var rawValue = CleanValue(line[(colonIndex + 1)..]);

            // Handle special "Basis: First Class, Boekhandelaar" composite line
            if (string.Equals(rawKey, "Basis", StringComparison.OrdinalIgnoreCase))
            {
                var parts = rawValue.Split(',', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length > 0 && !IsPlaceholder(parts[0]))
                {
                    items.Add(new EntityMetaItem("👑", "Klasse", parts[0]));
                }
                if (parts.Length > 1 && !IsPlaceholder(parts[1]))
                {
                    items.Add(new EntityMetaItem("💼", "Beroep", parts[1]));
                }
                continue;
            }

            if (IsPlaceholder(rawValue))
                continue;

            var (icon, label) = MapKey(rawKey, entityType);
            if (label is not null && !items.Any(i => string.Equals(i.Label, label, StringComparison.OrdinalIgnoreCase)))
            {
                items.Add(new EntityMetaItem(icon, label, rawValue));
            }
        }

        return items;
    }

    private static (string Icon, string? Label) MapKey(string key, CampaignEntityType entityType)
    {
        var normalized = key.ToLowerInvariant().Replace("*", "").Trim();

        return normalized switch
        {
            "klasse" or "social class" => ("👑", "Klasse"),
            "beroep" or "titel" or "beroep/titel" or "job" or "profession" => ("💼", "Beroep"),
            "ras" or "race" or "species" => ("🧬", "Ras"),
            "factie" or "faction" or "alliantie" => ("🚩", "Factie"),
            "type npc" or "type" when entityType == CampaignEntityType.Npc => ("🏷️", "Type"),
            "speler" or "player" or "speler (player name)" => ("🎮", "Speler"),
            "class" or "klasse (d&d)" when entityType == CampaignEntityType.Pc => ("⚔️", "Class"),
            "subklasse" or "subclass" or "archetype" => ("🛡️", "Subklasse"),
            "level" => ("⭐", "Level"),
            "type" when entityType == CampaignEntityType.Location => ("🏷️", "Type"),
            "regio" or "region" => ("🗺️", "Regio"),
            "sfeer" or "atmosphere" => ("✨", "Sfeer"),
            "beheerder" or "heerser" or "beheerder/heerser" or "ruler" => ("👑", "Heerser"),
            "type" when entityType == CampaignEntityType.Item => ("🏷️", "Type"),
            "zeldzaamheid" or "rarity" => ("💎", "Zeldzaamheid"),
            "attunement" => ("🔮", "Attunement"),
            "waarde" or "value" => ("💰", "Waarde"),
            _ => (string.Empty, null)
        };
    }

    private static string CleanValue(string value)
    {
        return value.Trim().Trim('*', '_', ' ').Trim();
    }

    private static bool IsPlaceholder(string value)
    {
        var v = value.Trim().ToLowerInvariant();
        return v is "" or "n/a" or "geen" or "none" or "-" or "..." or "selecteer klasse...";
    }
}
