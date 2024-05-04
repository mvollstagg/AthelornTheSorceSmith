using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
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
    public float basePhysicalDefence;
    public float physicalDefenceVariability;
    public float baseMagicalDefence;
    public float magicalDefenceVariability;
    public ArmorItemType armorType;
    

    public ArmorItemDataSO() : base()
    {
        type = ItemType.Armor;
        maxStackSize = 1;
    }

    public (float, float) GetPhysicalDefenceRange() => (basePhysicalDefence - physicalDefenceVariability, basePhysicalDefence + physicalDefenceVariability);
    public float GetPhysicalDefence() => basePhysicalDefence + Random.Range(-physicalDefenceVariability, physicalDefenceVariability);
    public (float, float) GetMagicalDefenceRange() => (baseMagicalDefence - magicalDefenceVariability, baseMagicalDefence + magicalDefenceVariability);
    public float GetMagicalDefence() => baseMagicalDefence + Random.Range(-magicalDefenceVariability, magicalDefenceVariability);

    public override string GetDetailsDisplay()
    {
        (float minPhysicalDef, float maPhysicalDef) = GetPhysicalDefenceRange();
        (float minMagicalDef, float maxMagicalDef) = GetMagicalDefenceRange();
        string output = base.GetDetailsDisplay();
        output += $"Physical Defence: {minPhysicalDef.ToString("0.0")} - {maPhysicalDef.ToString("0.0")}\n";
        output += $"Magical Defence: {minMagicalDef.ToString("0.0")} - {maxMagicalDef.ToString("0.0")}";
        return output;
    }

    public override string GetTypesDisplay()
    {
        string output = base.GetTypesDisplay();
        output += $"{armorType.GetDescription()}";
        return output;
    }
}