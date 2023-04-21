using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    public static class ItemRarityColors
    {
        private static Color Common = Color.white;
        private static Color Uncommon = new Color(0.1f, 1f, 0.1f, 1f);
        private static Color Rare = new Color(0.1f, 0.1f, 1f, 1f);
        private static Color Epic = new Color(0.6f, 0.1f, 0.6f, 1f);
        private static Color Legendary = new Color(0.6f, 0.6f, 0.1f, 1f);

        public static Color GetColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return Common;
                case ItemRarity.Uncommon:
                    return Uncommon;
                case ItemRarity.Rare:
                    return Rare;
                case ItemRarity.Epic:
                    return Epic;
                case ItemRarity.Legendary:
                    return Legendary;
                default:
                    return Common;
            }
        }
    }
}