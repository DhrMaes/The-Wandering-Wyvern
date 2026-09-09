namespace DhrMaes.WanderingWyvern.Web.Client.Models;

public record EntityCreateResult(
    string Title,
    string FolderName,
    // NPC fields
    bool IsNamedNpc = false,
    string NpcClass = "",
    string NpcJob = "",
    string NpcRace = "",
    string NpcFaction = "",
    // PC fields
    string PlayerName = "",
    string PcRace = "",
    string PcClass = "",
    string PcSubclass = "",
    string PcLevel = "",
    // Location fields
    string LocationType = "",
    string LocationRegion = "",
    string LocationAtmosphere = "",
    string LocationRuler = "",
    // Lore fields
    string LoreCategory = "",
    string LoreEra = "",
    string LoreEntities = "",
    // Handout fields
    string HandoutType = "",
    string HandoutAuthor = "",
    string HandoutRecipient = "",
    string HandoutLanguage = "",
    // Session fields
    string SessionDate = "",
    string SessionLocation = "",
    string SessionObjective = "",
    // Item common fields
    string ItemType = "",
    string ItemRarity = "",
    bool RequiresAttunement = false,
    string AttunementDetails = "",
    string ItemValue = "",
    // Item Weapon fields
    string WeaponCategory = "",
    string WeaponDamage = "",
    string WeaponDamageType = "",
    string WeaponProperties = "",
    string WeaponRange = "",
    // Item Armor fields
    string ArmorCategory = "",
    string ArmorAc = "",
    string StrengthRequirement = "",
    bool StealthDisadvantage = false,
    // Item Potion fields
    string PotionForm = "",
    string PotionEffect = "",
    string PotionDuration = "",
    // Item Ring / Wand / Staff / Wondrous fields
    string ItemSlot = "",
    string ItemCharges = "",
    string ItemRecharge = "",
    string ItemSpellsAbilities = "",
    string ItemEffect = "",
    // Item Scroll fields
    string ScrollSpellName = "",
    string ScrollSpellLevel = "",
    string ScrollMagicSchool = "",
    // Item Material fields
    string MaterialCategory = "",
    string MaterialSource = "",
    string CraftingUsage = "",
    // Item Trinket fields
    string TrinketOrigin = "",
    string TrinketQuirk = ""
);

