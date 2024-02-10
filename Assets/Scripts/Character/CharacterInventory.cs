using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
public class CharacterInventory : Singleton<CharacterInventory>
{
    public Dictionary<int, InventorySlot> _inventory = new Dictionary<int, InventorySlot>();
    
    [Header("Drag and Drop")]
    public int _grabbedInventoryItemSlotIndex = -1;

    public Dictionary<int, InventorySlot> InventoryItems => _inventory;
    public List<InventoryDetails> _inventorySlots = new List<InventoryDetails>();

    public int GrabbedInventoryItemSlotIndex => _grabbedInventoryItemSlotIndex;

    private void LateUpdate()
    {
        _inventorySlots = _inventory.Keys.Select(x => new InventoryDetails { Index = x, Name = _inventory[x].Item.name }).ToList();
    }

    public void DisableInventory() // OnInventoryDisabled Listener -ish //
    {
        _grabbedInventoryItemSlotIndex = -1;
    }
    public void EnableInventory()
    {
        _grabbedInventoryItemSlotIndex = 0;
    }
    private void _SwapInventoryItems(int grabbedSlotIndex, int slotIndex)
    {
        InventorySlot grabbedItem = _inventory[grabbedSlotIndex];
        InventorySlot swappedItem = _inventory[slotIndex];

        _inventory[grabbedSlotIndex] = swappedItem;
        _inventory[slotIndex] = grabbedItem;

        if (grabbedItem.Item.code == swappedItem.Item.code && grabbedItem.Item.maxStackSize > 1)
        {
            int totalAmount = grabbedItem.Amount + swappedItem.Amount;
            int stackCount = Mathf.Min(grabbedItem.Item.maxStackSize, totalAmount);

            _inventory[grabbedSlotIndex] = new InventorySlot
            {
                Item = grabbedItem.Item,
                Amount = stackCount
            };

            _inventory[slotIndex] = new InventorySlot
            {
                Item = swappedItem.Item,
                Amount = totalAmount - stackCount
            };
        }
        _grabbedInventoryItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowInventoryItemDetails(slotIndex);
    }
    public int _GetNextSlotIndex()
    {
        if (CharacterInventory.Instance._inventory.Count == 0) return 0;

        int index = 0;
        while (CharacterInventory.Instance._inventory.ContainsKey(index))
        {
            index++;
        }
        return index;
    }
    public void GrabInventoryItem(int slotIndex)
    {
        if (InventoryManager.Instance._grabbedEquipmentItemSlotIndex != -1)
        {
            // Equip the item logic
            InventorySlot item = InventoryManager.Instance._equipments[InventoryManager.Instance._grabbedEquipmentItemSlotIndex];
            InventoryManager.Instance._DropGrabbedEquipmentItem(InventoryManager.Instance._grabbedEquipmentItemSlotIndex, slotIndex, item);
        }
        else
        {
            if (_grabbedInventoryItemSlotIndex == slotIndex)
            {
                // If the grabbed item is the same as the one being grabbed, drop it
                _DropGrabbedInventoryItem(slotIndex);
            }
            else if (_grabbedInventoryItemSlotIndex != -1)
            {
                // If there is a different item already grabbed, swap them or drop the grabbed item
                if (_inventory.ContainsKey(slotIndex))
                {
                    _SwapInventoryItems(_grabbedInventoryItemSlotIndex, slotIndex);
                }
                else
                {
                    _DropGrabbedInventoryItem(slotIndex);
                }
            }
            else if (_inventory.ContainsKey(slotIndex))
            {
                EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemGrab });

                // If there is no item grabbed, grab the item
                _grabbedInventoryItemSlotIndex = slotIndex;
                InventorySlot inventorySlot = _inventory[slotIndex];
                InventoryUIManager.Instance.SetGrabItemSlot(inventorySlot);
                InventoryUIManager.Instance.SetGrabbedItemSlotStatus(true);
                InventoryUIManager.Instance.SetInventorySlotIconOpacity(_grabbedInventoryItemSlotIndex, 0.25f);
            }
        }

        InventoryUIManager.Instance.UpdateInventoryGrids();
        InventoryUIManager.Instance.UpdateEquipmentGrids();
    }

    public void RemoveInventoryItem(int slotIndex, int amount = 1)
    {
        if (!_inventory.TryGetValue(slotIndex, out InventorySlot slot)) return;

        if (amount == -1)
            amount = slot.Amount;

        slot.Amount -= amount;

        if (slot.Amount <= 0)
        {
            _inventory.Remove(slotIndex);
        }

        // Multiply by -1 to remove weight
        InventoryUIManager.Instance.SetInventoryWeight(slot.Item.weight * (slot.Amount >= 0 ? amount : slot.Amount) * -1);
        InventoryUIManager.Instance.ShowInventoryItemDetails(slotIndex);
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    public void _DropGrabbedInventoryItem(int slotIndex)
    {
        InventorySlot grabbedItem = _inventory[_grabbedInventoryItemSlotIndex];

        // Update the index of the grabbed item
        _inventory.Remove(_grabbedInventoryItemSlotIndex);
        _inventory.Add(slotIndex, grabbedItem);

        _grabbedInventoryItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowInventoryItemDetails(slotIndex);
    }

    public void AddMoney(int amount)
    {
        Character.Instance.money += amount;
        InventoryUIManager.Instance.SetInventoryMoney();
    }
}