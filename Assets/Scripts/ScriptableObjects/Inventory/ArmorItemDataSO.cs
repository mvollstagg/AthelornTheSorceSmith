using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Enum;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Armor",
    menuName = "Scriptable Objects/Inventory/Armor"
)]
public class ArmorItemDataSO : InventoryItemDataSO
{
    [Header("Armor Data")]
    public float baseDefence;
    public float defenceVariability;
    

    public ArmorItemDataSO() : base()
    {
        type = ItemType.Armor;
        maxStackSize = 1;
    }

    public (float, float) GetDefenceRange()
        => (baseDefence - defenceVariability, baseDefence + defenceVariability);

    public float GetDefence()
        => baseDefence + Random.Range(-defenceVariability, defenceVariability);
}