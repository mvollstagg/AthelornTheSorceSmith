using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum ItemType
    {
        [Description("Weapon")]
        Weapon,
        [Description("Armor")]
        Armor,
        [Description("Component")]
        Component,
        [Description("Miscellaneous")]
        Miscellaneous
    }
}