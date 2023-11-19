using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    [Serializable]
    public class InventorySlot
    {
        public InventoryItemDataSO Item;
        public int Amount;
    }
}