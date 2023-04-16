using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Enum;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Item", 
    menuName = "Scriptable Objects/Inventory/Item"
)]
public class InventoryItemDataSO : ScriptableObject
{
    public string code;
    public string itemName;
    public Sprite icon;
    public float weight;
    public int price;
    public int maxStackSize;

    public static Dictionary<ItemRarity, Color> ITEM_RARITY_COLORS
        = new Dictionary<ItemRarity, Color>()
        {
            { ItemRarity.Common, Color.white },
            { ItemRarity.Uncommon, new Color(0.1f, 1f, 0.1f, 1f) },
            { ItemRarity.Rare, new Color(0.1f, 0.1f, 1f, 1f) },
            { ItemRarity.Epic, new Color(0.6f, 0.1f, 0.6f, 1f) },
            { ItemRarity.Legendary, new Color(0.6f, 0.6f, 0.1f, 1f) },
        };

    public ItemRarity rarity;
    public ItemType type;
}