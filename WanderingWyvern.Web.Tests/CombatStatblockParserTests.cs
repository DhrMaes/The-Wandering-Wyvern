using DhrMaes.WanderingWyvern.Core.Models;
using DhrMaes.WanderingWyvern.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
public sealed class CombatStatblockParserTests
{
    private const string SampleHookHorrorStatblock = """
        # Hook Horror
        *Large monstrosity, unaligned*

        - **Armor Class:** 15 (natural armor)
        - **Hit Points:** 75 (10d10 + 20)
        - **Speed:** 30 ft., climb 30 ft.

        | STR | DEX | CON | INT | WIS | CHA |
        |:---:|:---:|:---:|:---:|:---:|:---:|
        | 18 (+4) | 10 (+0) | 15 (+2) | 6 (-2) | 12 (+1) | 7 (-2) |

        ### Eigenschappen
        - **Echolocation:** The hook horror can't use its blindsight while deafened.
        - **Keen Hearing:** The hook horror has advantage on Wisdom (Perception) checks that rely on hearing.

        ### Acties
        - **Multiattack:** The hook horror makes two hook attacks.
        - **Hook:** *Melee Weapon Attack:* +6 to hit, reach 10 ft., one target. *Hit:* 11 (2d6 + 4) piercing damage.

        ### Reacties
        - **Uncanny Screech:** When taking damage, the hook horror emits a piercing screech.
        """;

    [TestMethod]
    public void ExtractStats_ParsesAcHpAndSpeedCorrectly()
    {
        var (ac, hp, speed) = CombatStatblockParser.ExtractStats(SampleHookHorrorStatblock);

        Assert.AreEqual(15, ac);
        Assert.AreEqual(75, hp);
        Assert.AreEqual("30 ft., climb 30 ft", speed);
    }

    [TestMethod]
    public void ExtractActions_ExtractsActionsTraitsAndReactions()
    {
        var actions = CombatStatblockParser.ExtractActions(SampleHookHorrorStatblock);

        Assert.IsTrue(actions.Count >= 4);

        // Traits
        var echolocation = actions.FirstOrDefault(a => a.Name == "Echolocation");
        Assert.IsNotNull(echolocation);
        Assert.AreEqual("Eigenschap", echolocation.Type);

        // Multiattack
        var multiattack = actions.FirstOrDefault(a => a.Name == "Multiattack");
        Assert.IsNotNull(multiattack);
        Assert.AreEqual("Actie", multiattack.Type);

        // Hook attack with +6 to hit and 2d6 + 4 piercing damage
        var hook = actions.FirstOrDefault(a => a.Name == "Hook");
        Assert.IsNotNull(hook);
        Assert.AreEqual("Actie", hook.Type);
        Assert.AreEqual("+6 to hit", hook.AttackBonus);
        Assert.IsNotNull(hook.Damage);
        StringAssert.Contains(hook.Damage, "2d6 + 4");

        // Reaction
        var screech = actions.FirstOrDefault(a => a.Name == "Uncanny Screech");
        Assert.IsNotNull(screech);
        Assert.AreEqual("Reactie", screech.Type);
    }

    [TestMethod]
    public void Participant_ApplyDamageAndHealing_CalculatesHpAccurately()
    {
        var p = new CombatParticipant
        {
            Name = "Thorkel",
            MaxHp = 40,
            CurrentHp = 40,
            TempHp = 10,
            ArmorClass = 16
        };

        // 5 damage absorbed by TempHP
        p.ApplyDamage(5);
        Assert.AreEqual(5, p.TempHp);
        Assert.AreEqual(40, p.CurrentHp);

        // 10 damage: absorbs 5 TempHP, 5 HP loss
        p.ApplyDamage(10);
        Assert.AreEqual(0, p.TempHp);
        Assert.AreEqual(35, p.CurrentHp);
        Assert.AreEqual(87.5, p.GetHpPercentage());

        // Healing
        p.ApplyHealing(10);
        Assert.AreEqual(40, p.CurrentHp);

        // Toggle condition
        p.ToggleCondition("Poisoned");
        Assert.IsTrue(p.Conditions.Contains("Poisoned"));
        p.ToggleCondition("Poisoned");
        Assert.IsFalse(p.Conditions.Contains("Poisoned"));
    }
}
