using System.Text.RegularExpressions;
using DhrMaes.WanderingWyvern.Core.Models;

namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Parses 5e statblock details (Armor Class, Hit Points, Speed, and Actions/Attacks) from creature Markdown sheets.
/// </summary>
public static class CombatStatblockParser
{
    private static readonly Regex ArmorClassRegex = new(
        @"(?:\*\*|__)?(?:Armor\s*Class|AC|Pantserklasse)[:.]?(?:\*\*|__)?[:\s]+(?<ac>\d+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex HitPointsRegex = new(
        @"(?:\*\*|__)?(?:Hit\s*Points|HP|Levenspunten)[:.]?(?:\*\*|__)?[:\s]+(?<hp>\d+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SpeedRegex = new(
        @"(?:\*\*|__)?(?:Speed|Snelheid)[:.]?(?:\*\*|__)?[:\s]+(?<speed>[^\r\n|]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SectionHeadingRegex = new(
        @"^(?<level>#{2,4})\s+(?<title>[^\r\n]+)",
        RegexOptions.Multiline | RegexOptions.Compiled);

    // Matches e.g. ***Bite.*** or **Bite.** or - **Bite:** or **Bite:**
    private static readonly Regex ActionItemRegex = new(
        @"(?:^|\r?\n)(?:[-*]\s+)?(?:\*{2,3}|_{2,3})(?<name>[^*_:\r\n]+)[:.]?(?:\*{2,3}|_{2,3})[:.]?\s*(?<bodycontent>[^\r\n]+(?:\r?\n(?!(?:[-*]\s+)?(?:\*{2,3}|_{2,3})|#{2,4}\s)[^\r\n]+)*)",
        RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex ToHitRegex = new(
        @"(?<tohit>[+-]\d+)\s*(?:to\s*hit|om\s*te\s*raken)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex DamageRegex = new(
        @"(?:\*|_)*(?:Hit|Raak|Schade)[:.]*(?:\*|_)*\s*(?:(?:\d+\s*\((?<dice>[^)]+)\)|\b(?<dice>\d+d\d+(?:\s*[+-]\s*\d+)?))\s*(?<type>[a-zA-Z]+)?(?:\s*(?:damage|schade))?|\b(?<dice>\d+d\d+(?:\s*[+-]\s*\d+)?)\s*(?<type>[a-zA-Z]+)?(?:\s*(?:damage|schade))?)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses core stats (Armor Class, Max HP, and Speed) from Markdown text.
    /// </summary>
    public static (int ArmorClass, int HitPoints, string Speed) ExtractStats(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return (10, 10, "30 ft.");

        var ac = 10;
        var hp = 10;
        var speed = "30 ft.";

        var acMatch = ArmorClassRegex.Match(markdown);
        if (acMatch.Success && int.TryParse(acMatch.Groups["ac"].Value, out var parsedAc))
        {
            ac = parsedAc;
        }

        var hpMatch = HitPointsRegex.Match(markdown);
        if (hpMatch.Success && int.TryParse(hpMatch.Groups["hp"].Value, out var parsedHp))
        {
            hp = parsedHp;
        }

        var speedMatch = SpeedRegex.Match(markdown);
        if (speedMatch.Success)
        {
            speed = speedMatch.Groups["speed"].Value.Trim().TrimEnd('.', ',');
        }

        return (ac, hp, speed);
    }

    /// <summary>
    /// Parses all combat actions, bonus actions, reactions, and special traits from Markdown.
    /// </summary>
    public static List<CombatActionInfo> ExtractActions(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return [];

        var actions = new List<CombatActionInfo>();
        var sections = SplitIntoSections(markdown);

        foreach (var (sectionTitle, sectionContent) in sections)
        {
            var actionType = DetermineActionType(sectionTitle);
            if (actionType is null)
                continue;

            var matches = ActionItemRegex.Matches(sectionContent);
            foreach (Match match in matches)
            {
                var name = match.Groups["name"].Value.Trim().TrimEnd('.', ':');
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var body = match.Groups["bodycontent"].Value.Trim();
                if (string.IsNullOrWhiteSpace(body))
                    continue;

                // Check for attack bonus and damage dice
                string? attackBonus = null;
                var toHitMatch = ToHitRegex.Match(body);
                if (toHitMatch.Success)
                {
                    attackBonus = $"{toHitMatch.Groups["tohit"].Value} to hit";
                }

                string? damage = null;
                var damageMatch = DamageRegex.Match(body);
                if (damageMatch.Success)
                {
                    var dice = damageMatch.Groups["dice"].Value.Trim();
                    var type = damageMatch.Groups["type"].Value.Trim();
                    damage = string.IsNullOrWhiteSpace(type) ? dice : $"{dice} {type}".Trim();
                }

                actions.Add(new CombatActionInfo(
                    Name: name,
                    Type: actionType,
                    Description: body,
                    AttackBonus: attackBonus,
                    Damage: damage
                ));
            }
        }

        return actions;
    }

    private static List<(string Title, string Content)> SplitIntoSections(string markdown)
    {
        var result = new List<(string Title, string Content)>();
        var matches = SectionHeadingRegex.Matches(markdown);

        for (var i = 0; i < matches.Count; i++)
        {
            var current = matches[i];
            var title = current.Groups["title"].Value.Trim();
            var startIndex = current.Index + current.Length;
            var endIndex = (i + 1 < matches.Count) ? matches[i + 1].Index : markdown.Length;
            var content = markdown[startIndex..endIndex];
            result.Add((title, content));
        }

        return result;
    }

    private static string? DetermineActionType(string heading)
    {
        var lower = heading.ToLowerInvariant();
        if (lower.Contains("bonus"))
            return "Bonus Actie";
        if (lower.Contains("reaction") || lower.Contains("reactie"))
            return "Reactie";
        if (lower.Contains("legendary") || lower.Contains("legendarisch"))
            return "Legendarisch";
        if (lower.Contains("action") || lower.Contains("actie"))
            return "Actie";
        if (lower.Contains("trait") || lower.Contains("eigenschap") || lower.Contains("special"))
            return "Eigenschap";

        return null;
    }
}
