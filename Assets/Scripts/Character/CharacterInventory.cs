using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using UnityEngine;

public class CharacterInventory : Singleton<CharacterInventory>
{
    public int EquippedWeaponId;
    public GameObject RightHand;
    public GameObject RightHandWeaponPrefab;

    public void ItemEquipped()
    {
        var equippedItem = InventoryManager
                            .Instance
                            .EquipmentItems
                            .Where(p => p.Value.Item.equipmentSlotType == Scripts.Entities.Enum.EquipmentSlotType.RightHand)
                            .FirstOrDefault();

        if (equippedItem.Value == null)
        {
            EquippedWeaponId = 0;
            EquipWeapon();
            return;
        }
        EquippedWeaponId = equippedItem.Value.Item.id;
        EquipWeapon();
    }

    public void EquipWeapon()
    {
        if (RightHandWeaponPrefab != null)
        {
            Destroy(RightHandWeaponPrefab);
        }

        var weapon = InventoryManager
                     .Instance
                     .EquipmentItems
                     .Where(p => p.Value.Item.id == EquippedWeaponId)
                     .FirstOrDefault();

        if (weapon.Value == null)
        {
            return;
        }

        RightHandWeaponPrefab = Instantiate(weapon.Value.Item.prefab, RightHand.transform.position, RightHand.transform.rotation);
        RightHandWeaponPrefab.transform.SetParent(RightHand.transform);
    }
}
