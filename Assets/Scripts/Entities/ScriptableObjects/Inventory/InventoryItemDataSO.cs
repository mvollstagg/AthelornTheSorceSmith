using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Enum;
using UnityEngine;

// [CreateAssetMenu
// (
//     fileName = "Item", 
//     menuName = "Scriptable Objects/Inventory/Item"
// )]
public class InventoryItemDataSO : ScriptableObject
{
    public string code;
    public string itemName;
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;
    public float weight;
    public int price;
    public int maxStackSize;
    public ItemRarity rarity;
    public ItemType type;
}