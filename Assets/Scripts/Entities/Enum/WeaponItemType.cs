using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum WeaponItemType
    {
        [Description("One Handed Sword")]
        OneHandedSword,
        [Description("Two Handed Sword")]
        TwoHandedSword,
        [Description("One Handed Axe")]
        OneHandedAxe,
        [Description("Two Handed Axe")]
        TwoHandedAxe,
        [Description("One Handed Mace")]
        OneHandedMace,
        [Description("Two Handed Mace")]
        TwoHandedMace,
        [Description("Bow")]
        Bow,
        [Description("Crossbow")]
        Crossbow,
        [Description("Staff")]
        Staff,
        [Description("Dagger")]
        Dagger,
        [Description("Shield")]
        Shield,
        [Description("Wand")]
        Wand,
        [Description("Tool")]
        Tool
    }
}