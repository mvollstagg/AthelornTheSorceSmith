using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Enum;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Weapon",
    menuName = "Scriptable Objects/Inventory/Weapon"
)]
public class WeaponItemDataSO : InventoryItemDataSO
{
    [Header("Weapon Data")]
    public float baseDamage;
    public float damageVariability;
    public float criticalStrikeChance;
    public WeaponItemType weaponType;

    public WeaponItemDataSO() : base()
    {
        type = ItemType.Weapon;
        maxStackSize = 1;
    }

    public (float, float) GetDamageRange() => (baseDamage - damageVariability, baseDamage + damageVariability);

    public float GetDamage()
    {
        float dmg = baseDamage + Random.Range(-damageVariability, damageVariability);
        return (Random.Range(0f, 1f) < criticalStrikeChance) ? dmg : dmg * 2;
    }
}