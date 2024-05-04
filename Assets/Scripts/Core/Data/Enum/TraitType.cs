using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum TraitType
    {
        [Description("Strength")]
        Strength,
        [Description("Dexterity")]
        Dexterity,
        [Description("Intelligence")]
        Intelligence,
        [Description("Vitality")]
        Vitality,
        [Description("Focus")]
        Focus,
        [Description("Charisma")]
        Charisma,

        [Description("Pysical Damage")]
        PhysicalDamage,
        [Description("Pysical Defence")]
        PhysicalDefence,
        [Description("Magical Damage")]
        MagicalDamage,
        [Description("Magical Defence")]
        MagicalDefence,

        [Description("Health")]
        Health,
        [Description("Mana")]
        Mana,
        [Description("Stamina")]
        Stamina,

        [Description("Critical Strike Chance")]
        CriticalStrikeChance,
        [Description("Critical Strike Damage")]
        CriticalStrikeDamage,
        [Description("Accuracy")]
        Accuracy,
        [Description("Dodge Chance")]
        DodgeChance,
        [Description("Block Chance")]
        BlockChance,
        [Description("Influence Bonus")]
        InfluenceBonus,
        [Description("Negotiation Bonus")]
        NegotiationBonus,

        [Description("Fire Resistance")]
        FireResistance,
        [Description("Water Resistance")]
        WaterResistance,
        [Description("Earth Resistance")]
        EarthResistance,
        [Description("Air Resistance")]
        AirResistance,
        [Description("Poison Resistance")]
        PoisonResistance
    }
}