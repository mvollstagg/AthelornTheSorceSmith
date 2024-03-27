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
            return;
        }
        EquippedWeaponId = equippedItem.Value.Item.id;
    }
}
