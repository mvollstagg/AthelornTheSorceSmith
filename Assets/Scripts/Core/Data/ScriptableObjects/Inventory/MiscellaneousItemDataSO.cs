using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Enum;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Miscellaneous",
    menuName = "Scriptable Objects/Inventory/Miscellaneous"
)]
public class MiscellaneousItemDataSO : InventoryItemDataSO
{
    // [Header("Miscellaneous Data")]
    // public MiscellaneousType miscellaneousType;
    // public MiscellaneousEffectType miscellaneousEffectType;
    public MiscellaneousItemDataSO() : base()
    {
        type = ItemType.Miscellaneous;
    }
}
