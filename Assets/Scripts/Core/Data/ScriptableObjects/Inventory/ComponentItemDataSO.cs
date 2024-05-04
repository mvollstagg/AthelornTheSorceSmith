using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Enum;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Component",
    menuName = "Scriptable Objects/Inventory/Component"
)]
public class ComponentItemDataSO : InventoryItemDataSO
{
    // [Header("Component Data")]
    // public ComponentType componentType;
    // public CraftingComponentType craftingComponentType;
    // public List<CraftingComponentType> neededComponents;
    public ComponentItemDataSO() : base()
    {
        type = ItemType.Component;
    }
}
