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
    public float ItemEquipped()
    {
        var isEquipped = InventoryManager.Instance.EquipmentItems.Any(p => p.Value.Item.equipmentSlotType == Scripts.Entities.Enum.EquipmentSlotType.RightHand);
        
        return isEquipped ? 1 : 0;
    }
}
