namespace DhrMaes.WanderingWyvern.Core.Models;

/// <summary>
/// Represents the overall state of an ongoing battle encounter.
/// </summary>
public sealed class CombatState
{
    public bool IsActive { get; set; }
    public int Round { get; set; } = 1;
    public int ActiveParticipantIndex { get; set; } = 0;
    public List<CombatParticipant> Participants { get; set; } = [];

    public CombatParticipant? GetActiveParticipant()
    {
        if (Participants.Count == 0 || ActiveParticipantIndex < 0 || ActiveParticipantIndex >= Participants.Count)
            return null;
        return Participants[ActiveParticipantIndex];
    }
}

/// <summary>
/// Represents a creature or character participating in a combat encounter.
/// </summary>
public sealed class CombatParticipant
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string? EntityRelativePath { get; set; }
    public bool IsPlayerCharacter { get; set; }

    // Core combat statistics
    public int Initiative { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int TempHp { get; set; }
    public int ArmorClass { get; set; }
    public string? Speed { get; set; }

    // Status & conditions
    public bool IsDefeated => CurrentHp <= 0;
    public List<string> Conditions { get; set; } = [];
    public int DeathSaveSuccesses { get; set; }
    public int DeathSaveFailures { get; set; }

    // Actions parsed from sheet (attacks, traits, spells)
    public List<CombatActionInfo> Actions { get; set; } = [];

    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;

        if (TempHp > 0)
        {
            if (amount <= TempHp)
            {
                TempHp -= amount;
                return;
            }
            amount -= TempHp;
            TempHp = 0;
        }

        CurrentHp = Math.Max(0, CurrentHp - amount);
    }

    public void ApplyHealing(int amount)
    {
        if (amount <= 0) return;
        CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
        if (CurrentHp > 0)
        {
            DeathSaveSuccesses = 0;
            DeathSaveFailures = 0;
        }
    }

    public void ToggleCondition(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition)) return;
        var existing = Conditions.FirstOrDefault(c => string.Equals(c, condition, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            Conditions.Remove(existing);
        }
        else
        {
            Conditions.Add(condition);
        }
    }

    public double GetHpPercentage()
    {
        if (MaxHp <= 0) return 0;
        return Math.Clamp((double)CurrentHp / MaxHp * 100.0, 0, 100);
    }
}

/// <summary>
/// A parsed action, attack, or ability of a creature.
/// </summary>
public sealed record CombatActionInfo(
    string Name,
    string Type, // "Aanval", "Actie", "Bonus Actie", "Reactie", "Eigenschap"
    string Description,
    string? AttackBonus = null, // e.g. "+5 to hit"
    string? Damage = null       // e.g. "2d6 + 3 slashing"
);
