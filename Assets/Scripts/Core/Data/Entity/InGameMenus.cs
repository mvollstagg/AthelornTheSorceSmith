using System;
using Scripts.Entities.Enum;

namespace Scripts.Entities.Class
{
    public static class InGameMenus
    {
        public static string INVENTORY { get; private set; } = "Inventory";
        public static string CHARACTER { get; private set; } = "Character";
        public static string SKILLS { get; private set; } = "Skills";
        public static string CRAFTING { get; private set; } = "Crafting";
        public static string SOCIAL { get; private set; } = "Social";
        public static string QUESTS { get; private set; } = "Quests";
        public static string MAP { get; private set; } = "Map";
        public static string SETTINGS { get; private set; } = "Settings";
        
    }
}