using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
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
    public GameObject prefab;
    public float weight;
    public int price;
    public int maxStackSize;
    public int durability;
    public ItemRarity rarity;
    public ItemType type;
    public EquipmentSlotType equipmentSlotType = EquipmentSlotType.None;
    [SerializeField]
    public List<ItemTrait> traits = new List<ItemTrait>();

    public virtual string GetDetailsDisplay()
    {
        string output = $"Durability: {durability}%\n";
        return output;
    }

    public virtual string GetTraitsDisplay()
    {
        string output = "";
        foreach (ItemTrait trait in traits)
        {
            if (trait.Status == TraitStatus.Positive)
            {
                output += $"<color=#99ff66>{trait.Type.GetDescription()} +{trait.Value}\n";
            }
            else if (trait.Status == TraitStatus.Negative)
            {
                output += $"<color=#ff5050>{trait.Type.GetDescription()} -{trait.Value}\n";
            }
        }
        return output;
    }

    public virtual string GetTypesDisplay()
    {
        string output = "";
        output += $"<b>{type.GetDescription()}</b>\n";
        return output;
    }
}