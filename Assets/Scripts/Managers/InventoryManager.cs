using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

// Inventory Manager must contain only managing stuffs between UI and Character Inventory
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Drag and Drop")]
    public int _grabbedEquipmentItemSlotIndex = -1;
    private int _maxNumberOfSlots;

    [Header("Equipment Slots")]
    [SerializeField] public Dictionary<int, InventorySlot> _equipments = new Dictionary<int, InventorySlot>();
    public Dictionary<int, InventorySlot> EquipmentItems => _equipments;

    [Header("Active Slots")]
    [SerializeField] public List<InventoryDetails> _equipmentSlots = new List<InventoryDetails>(); 
    
    public int GrabbedEquipmentItemSlotIndex => _grabbedEquipmentItemSlotIndex;

    private void Start()
    {
        _maxNumberOfSlots = InventoryUIManager.Instance.ItemGridCount;
    }

    public void AddItem()
    {
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    public void OnInventoryDisabled()
    {
        InventoryUIManager.Instance.OnInventoryDisabled();
        CharacterInventory.Instance._grabbedInventoryItemSlotIndex = -1;
        _grabbedEquipmentItemSlotIndex = -1;
    }

    public void OnInventoryEnabled()
    {
        InventoryUIManager.Instance.OnInventoryEnabled();
    }

    // ! ######## Common ########

    

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

    public void RemoveEquipmentItem(int slotIndex)
    {
        if (!_equipments.TryGetValue(slotIndex, out InventorySlot slot)) return;

        _equipments.Remove(slotIndex);
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);

        InventoryUIManager.Instance.SetInventoryWeight(slot.Item.weight * -1);
        InventoryUIManager.Instance.ShowEquipmentItemDetails(slotIndex);
        InventoryUIManager.Instance.UpdateEquipmentGrids();
    }

    
    // ! ######## Input Methods ########
    public void OnDropItemAction()
    {
        if (_grabbedEquipmentItemSlotIndex != -1 || _grabbedInventoryItemSlotIndex != -1)
                return;

        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;

        if (InventoryUIManager.Instance.HoveredItemSlotType == HoveredItemSlotType.Inventory)
        {
            if (!_inventory.ContainsKey(selectedSlotIndex) || _grabbedInventoryItemSlotIndex != -1)
                return;

            LootManager.Instance.DropItem(new InventorySlot() { Item = _inventory[selectedSlotIndex].Item, Amount = 1 });
            RemoveInventoryItem(selectedSlotIndex);
        }
        else if (InventoryUIManager.Instance.HoveredItemSlotType == HoveredItemSlotType.Equipment)
        {
            if (!_equipments.ContainsKey(selectedSlotIndex) || _grabbedEquipmentItemSlotIndex != -1)
                return;

            LootManager.Instance.DropItem(new InventorySlot() { Item = _equipments[selectedSlotIndex].Item, Amount = 1 });
            RemoveEquipmentItem(selectedSlotIndex);
        }

        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemDrop });
    }   

    public void OnDropItemStackAction()
    {
        if (_grabbedEquipmentItemSlotIndex != -1 || _grabbedInventoryItemSlotIndex != -1)
                return;
                
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (InventoryUIManager.Instance.HoveredItemSlotType == HoveredItemSlotType.Inventory)
        {
            if (!_inventory.ContainsKey(selectedSlotIndex) || _grabbedInventoryItemSlotIndex != -1)
                return;

            LootManager.Instance.DropItem(new InventorySlot() { Item = _inventory[selectedSlotIndex].Item, Amount = _inventory[selectedSlotIndex].Amount });
            RemoveInventoryItem(selectedSlotIndex, -1);
        }
        else if (InventoryUIManager.Instance.HoveredItemSlotType == HoveredItemSlotType.Equipment)
        {
            if (!_equipments.ContainsKey(selectedSlotIndex) || _grabbedEquipmentItemSlotIndex != -1)
                return;

            LootManager.Instance.DropItem(new InventorySlot() { Item = _equipments[selectedSlotIndex].Item, Amount = _equipments[selectedSlotIndex].Amount });
            RemoveEquipmentItem(selectedSlotIndex);
        }

        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemDrop });
    }

    public void OnSortInventoryAction()
    {
        List<InventorySlot> sortedSlots = _inventory
            .Values
            .OrderByDescending((InventorySlot s) => s.Item.price * s.Amount)
            .ThenBy((InventorySlot s) => s.Item.name)
            .ToList();

        Dictionary<int, InventorySlot> newSlots = new Dictionary<int, InventorySlot>();
        for (int i = 0; i < sortedSlots.Count; i++)
            newSlots.Add(i, sortedSlots[i]);

        _inventory = newSlots;
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    
    // ! ######## Equipment ########
    public void GrabEquipmentItem(int slotIndex)
    {
        // TODO: If an item grabbed from inventory then equip it. If not then it is an equipment slot, so grab the item from there

        if (_grabbedInventoryItemSlotIndex != -1)
        {
            // Equip the item logic
            InventorySlot item = _inventory[_grabbedInventoryItemSlotIndex];
            _EquipItemDrag(_grabbedInventoryItemSlotIndex, slotIndex, item);
        }
        else
        {
            // Grab the item logic
            if (_grabbedEquipmentItemSlotIndex == slotIndex)
            {
                // If the grabbed item is the same as the one being grabbed, drop it
                _grabbedEquipmentItemSlotIndex = -1;
               CharacterInventory.Instance. _grabbedInventoryItemSlotIndex = -1;
                // InventoryUIManager.Instance.SetEquipmentSlotIconOpacity(_grabbedInventoryItemSlotIndex, 1f);
                InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
                InventoryUIManager.Instance.ShowEquipmentItemDetails(slotIndex);
            }
            else if (_grabbedEquipmentItemSlotIndex != -1)
            {
                // If there is a different item already grabbed and the slot is same slot type, swap the items
                if (_equipments[_grabbedEquipmentItemSlotIndex].Item.equipmentSlotType == InventoryUIManager.Instance.GetEquipmentSlotType(slotIndex))
                {
                    // Check if slot is empty. If so then move the item to the slot if not then swap the items
                    if (!_equipments.ContainsKey(slotIndex))
                    {
                        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemEquip });
                        _equipments.Add(slotIndex, _equipments[_grabbedEquipmentItemSlotIndex]);
                        _equipments.Remove(_grabbedEquipmentItemSlotIndex);
                        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);
                        _grabbedEquipmentItemSlotIndex = -1;
                        // InventoryUIManager.Instance.SetEquipmentSlotIconOpacity(_grabbedInventoryItemSlotIndex, 1f);
                        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
                        InventoryUIManager.Instance.ShowEquipmentItemDetails(slotIndex);
                    }
                    else
                    {
                        _SwapEquipmentItems(_grabbedEquipmentItemSlotIndex, slotIndex);
                    }
                }
            }
            else if (_equipments.ContainsKey(slotIndex))
            {
                EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemGrab });

                // If there is no item grabbed, grab the item
                _grabbedEquipmentItemSlotIndex = slotIndex;
                InventorySlot inventorySlot = _equipments[slotIndex];
                InventoryUIManager.Instance.SetGrabItemSlot(inventorySlot);
                InventoryUIManager.Instance.SetGrabbedItemSlotStatus(true);
                InventoryUIManager.Instance.SetEquipmentSlotIconOpacity(_grabbedEquipmentItemSlotIndex, 0.25f);
            }
        }

        InventoryUIManager.Instance.UpdateInventoryGrids();
        InventoryUIManager.Instance.UpdateEquipmentGrids();
    }

    public void QuickItemAction(int slotIndex)
    {
        if (!_inventory.ContainsKey(slotIndex))
            return;

        InventorySlot InventorySlot = _inventory[slotIndex];

        // Check if the item is an equipable item or consumable or not
        if (InventorySlot.Item.type == ItemType.Armor || InventorySlot.Item.type == ItemType.Weapon)
        {
            _QuickItemAction(slotIndex, _GetAvailableEquipmentSlot(InventorySlot), InventorySlot);
        }
        else if (InventorySlot.Item.type == ItemType.Potion || InventorySlot.Item.type == ItemType.Food)
        {            Character.Instance.ConsumeItem(InventorySlot.Item);
            RemoveInventoryItem(slotIndex);
        }
        else return;
    }

    private void _QuickItemAction(int fromEquipSlotIndex, int toEquipSlotIndex, InventorySlot item)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemEquip });

        // Check if the slot is empty. if not empty, unequip the item first before equipping the new one
        if (_equipments.ContainsKey(toEquipSlotIndex))
            _UnequipItem(toEquipSlotIndex);
            
        _equipments.Add(toEquipSlotIndex, new InventorySlot()
        {
            Item = item.Item,
            Amount = 1
        });
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);

        _inventory.Remove(fromEquipSlotIndex);
        InventoryUIManager.Instance.UpdateEquipmentGrids();
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    private void _EquipItemDrag(int fromEquipSlotIndex, int toEquipSlotIndex, InventorySlot item)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemEquip });

        // Check if grabbed Inventory item slot type and equipment slot type are the same
        if (item.Item.equipmentSlotType != InventoryUIManager.Instance.GetEquipmentSlotType(toEquipSlotIndex))
            return;
 
        // Check if the slot is empty. if not empty, unequip the item first before equipping the new one
        if (_equipments.ContainsKey(toEquipSlotIndex))
            _UnequipItem(toEquipSlotIndex);
            
        _equipments.Add(toEquipSlotIndex, new InventorySlot()
        {
            Item = item.Item,
            Amount = 1
        });
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);

        _grabbedEquipmentItemSlotIndex = -1;
        _grabbedInventoryItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowEquipmentItemDetails(toEquipSlotIndex);
        
        _inventory.Remove(fromEquipSlotIndex);
        InventoryUIManager.Instance.UpdateEquipmentGrids();
        InventoryUIManager.Instance.UpdateInventoryGrids();
        
    }
    
    public void UnequipItemQuick(int slotIndex)
    {
        if (!_equipments.ContainsKey(slotIndex))
            return;

        _UnequipItem(slotIndex);
    }

    private void _UnequipItem(int slotIndex)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemUnequip });

        _inventory.Add(_GetNextSlotIndex(), new InventorySlot()
        {
            Item = _equipments[slotIndex].Item,
            Amount = 1
        });
        _equipments.Remove(slotIndex);
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);
        InventoryUIManager.Instance.UpdateEquipmentGrids();
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    private void _SwapEquipmentItems(int grabbedSlotIndex, int slotIndex)
    {        
        InventorySlot grabbedItem = _equipments[grabbedSlotIndex];
        InventorySlot swappedItem = _equipments[slotIndex];

        _equipments[grabbedSlotIndex] = swappedItem;
        _equipments[slotIndex] = grabbedItem;

        if (grabbedItem.Item.code == swappedItem.Item.code && grabbedItem.Item.maxStackSize > 1)
        {
            int totalAmount = grabbedItem.Amount + swappedItem.Amount;
            int stackCount = Mathf.Min(grabbedItem.Item.maxStackSize, totalAmount);
            
            _equipments[grabbedSlotIndex] = new InventorySlot
            {
                Item = grabbedItem.Item,
                Amount = stackCount
            };
            
            _equipments[slotIndex] = new InventorySlot
            {
                Item = swappedItem.Item,
                Amount = totalAmount - stackCount
            };
        }

        _grabbedEquipmentItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowEquipmentItemDetails(slotIndex);
    }

    private void _DropGrabbedEquipmentItem(int fromSlotIndex, int toSlotIndex, InventorySlot item)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemUnequip });

        // Update the index of the grabbed item
        _equipments.Remove(fromSlotIndex);
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);
        // Check if clicked inventory slot is empty. If not then find first avaliable slot
        var avaliableIndex = _inventory.ContainsKey(toSlotIndex) ? _GetNextSlotIndex() : toSlotIndex;
        _inventory.Add(avaliableIndex, item);

        _grabbedInventoryItemSlotIndex = -1;
        _grabbedEquipmentItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowInventoryItemDetails(avaliableIndex);
    }

    private int _GetAvailableEquipmentSlot(InventorySlot item)
    {
        var equipmentSlotTransforms = InventoryUIManager.Instance._equipmentsGrid.GetComponentsInChildren<EquipmentSlotManager>(true);

        var matchingSlots = equipmentSlotTransforms
                                .Select((equipmentSlot, index) => new { EquipmentSlot = equipmentSlot, Index = index })
                                .Where(x => x.EquipmentSlot._slotType == item.Item.equipmentSlotType)
                                .Select(x => x.Index)
                                .ToList();
        
        if (matchingSlots.Count >= 2)
        {
            // If there are two matching slots, check if one of them is empty. If so, return the empty slot. If not return the first slot
            if (_equipments.ContainsKey(matchingSlots[0]))
                return matchingSlots[1];
            else
                return matchingSlots[0];
        }

        return matchingSlots.Count > 0 ? matchingSlots[0] : -1;
    }


    // ! ######## Inventory ########
    public void GrabInventoryItem(int slotIndex)
    {
        if (_grabbedEquipmentItemSlotIndex != -1)
        {
            // Equip the item logic
            InventorySlot item = _equipments[_grabbedEquipmentItemSlotIndex];
            _DropGrabbedEquipmentItem(_grabbedEquipmentItemSlotIndex, slotIndex, item);
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

    private void _DropGrabbedInventoryItem(int slotIndex)
    {
        InventorySlot grabbedItem = _inventory[_grabbedInventoryItemSlotIndex];
        
        // Update the index of the grabbed item
        _inventory.Remove(_grabbedInventoryItemSlotIndex);
        _inventory.Add(slotIndex, grabbedItem);

        _grabbedInventoryItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowInventoryItemDetails(slotIndex);
    }

    private int _GetNextSlotIndex()
    {
        if (_inventory.Count == 0) return 0;
    
        int index = 0;
        while (_inventory.ContainsKey(index))
        {
            index++;
        }
        return index;
    }

    public void AddMoney(int amount)
    {
        Character.Instance.money += amount;
        InventoryUIManager.Instance.SetInventoryMoney();
    }
}