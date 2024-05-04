using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum MiscelaneousItemType
    {
        [Description("Potion")]
        Potion,
        [Description("Food")]
        Food,
        [Description("Quest Item")]
        QuestItem,
        [Description("Crafting Item")]
        CraftingItem,
        [Description("Farming Item")]
        FarmingItem,
        [Description("Key")]
        Key,
    }
}