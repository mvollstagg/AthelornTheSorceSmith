using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
using Random = UnityEngine.Random;
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Drag and Drop")]
    public int _grabbedEquipmentItemSlotIndex = -1;
    public int _maxNumberOfSlots;
    public Dictionary<int, InventorySlot> _equipments = new Dictionary<int, InventorySlot>();

    [Header("Active Slots")]
    public List<InventoryDetails> _equipmentSlots = new List<InventoryDetails>();

    [Header("Test Items")]
    [SerializeField] private List<InventoryItemDataSO> _testItems;

    public Dictionary<int, InventorySlot> EquipmentItems => _equipments;
    
    public int GrabbedEquipmentItemSlotIndex => _grabbedEquipmentItemSlotIndex;

    private void Start()
    {
        _maxNumberOfSlots = InventoryUIManager.Instance.ItemGridCount;
        _maxNumberOfSlots = InventoryUIManager.Instance.ItemGridCount;
        _testItems.ForEach(x => AddItem(x, 1));
    }

    private void LateUpdate()
    {
        _equipmentSlots = _equipments.Keys.Select(x => new InventoryDetails { Index = x, Name = _equipments[x].Item.name }).ToList();
    }
    public void OnInventoryDisabled()
    {
        InventoryUIManager.Instance.OnInventoryDisabled();
        CharacterInventory.Instance.DisableInventory();
       
        _grabbedEquipmentItemSlotIndex = -1;
    }

    public void OnInventoryEnabled()
    {
        InventoryUIManager.Instance.OnInventoryEnabled();
        CharacterInventory.Instance.EnableInventory();
    }
    // ! ######## Common ########
    public void AddItem()
    {
        AddItem(_testItems[Random.Range(0, _testItems.Count)], 1);
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    public void AddItem(InventoryItemDataSO item, int amount = 1)
    {
        int remaining = amount;

        // Look for existing slots that can hold more items if item max stack size is greater than 1
        if (item.maxStackSize > 1)
        {
            foreach (var slot in CharacterInventory.Instance._inventory.Where(x => x.Value.Item.code == item.code))
            {
                // Calculate the amount of items that can be added to the slot without exceeding its max stack size
                int availableToAdd = slot.Value.Item.maxStackSize - slot.Value.Amount;

                // If we can add all remaining items to this slot, do so and exit the loop
                if (remaining <= availableToAdd)
                {
                    slot.Value.Amount += remaining;
                    remaining = 0;
                    break;
                }
                // Otherwise, fill the slot to its max stack size
                slot.Value.Amount += availableToAdd;
                remaining -= availableToAdd;
            }
        }

        // Fill existing slots to their maximum stack size before creating new slots
        while (remaining > 0)
        {
            var existingSlot = CharacterInventory.Instance._inventory.FirstOrDefault(slot => slot.Value.Item.code == item.code && slot.Value.Amount < slot.Value.Item.maxStackSize);

            if (existingSlot.Value != null)
            {
                int availableToAdd = existingSlot.Value.Item.maxStackSize - existingSlot.Value.Amount;
                int stackCount = Mathf.Min(availableToAdd, remaining);

                existingSlot.Value.Amount += stackCount;
                remaining -= stackCount;
            }
            else
            {
                // If there are no free slots, exit the loop
                if (CharacterInventory.Instance._inventory.Count >= _maxNumberOfSlots)
                    break;

                int stackCount = Mathf.Min(item.maxStackSize, remaining);

                CharacterInventory.Instance._inventory.Add(CharacterInventory.Instance._GetNextSlotIndex(), new InventorySlot()
                {
                    Item = item,
                    Amount = stackCount
                });

                remaining -= stackCount;
            }
        }
        // Update total weight of inventory
        float weightToAdd = item.weight * (amount - remaining);
        InventoryUIManager.Instance.SetInventoryWeight(weightToAdd);

        if (remaining > 0)
        {
            Debug.Log($"{remaining} {item.name} exceeded the maximum stack size.");
        }
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
        if (_grabbedEquipmentItemSlotIndex != -1 || CharacterInventory.Instance._grabbedInventoryItemSlotIndex != -1)
            return;

        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;

        if (InventoryUIManager.Instance.HoveredItemSlotType == HoveredItemSlotType.Inventory)
        {
            if (!CharacterInventory.Instance._inventory.ContainsKey(selectedSlotIndex) || CharacterInventory.Instance._grabbedInventoryItemSlotIndex != -1)
                return;

            LootManager.Instance.DropItem(new InventorySlot() { Item = CharacterInventory.Instance._inventory[selectedSlotIndex].Item, Amount = 1 });
            CharacterInventory.Instance.RemoveInventoryItem(selectedSlotIndex);
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
        if (_grabbedEquipmentItemSlotIndex != -1 || CharacterInventory.Instance._grabbedInventoryItemSlotIndex != -1)
            return;

        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (InventoryUIManager.Instance.HoveredItemSlotType == HoveredItemSlotType.Inventory)
        {
            if (!CharacterInventory.Instance._inventory.ContainsKey(selectedSlotIndex) || CharacterInventory.Instance._grabbedInventoryItemSlotIndex != -1)
                return;

            LootManager.Instance.DropItem(new InventorySlot() { Item = CharacterInventory.Instance._inventory[selectedSlotIndex].Item, Amount = CharacterInventory.Instance._inventory[selectedSlotIndex].Amount });
            CharacterInventory.Instance.RemoveInventoryItem(selectedSlotIndex, -1);
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
        List<InventorySlot> sortedSlots = CharacterInventory.Instance._inventory
            .Values
            .OrderByDescending((InventorySlot s) => s.Item.price * s.Amount)
            .ThenBy((InventorySlot s) => s.Item.name)
            .ToList();

        Dictionary<int, InventorySlot> newSlots = new Dictionary<int, InventorySlot>();
        for (int i = 0; i < sortedSlots.Count; i++)
            newSlots.Add(i, sortedSlots[i]);

        CharacterInventory.Instance._inventory = newSlots;
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }
    // ! ######## Equipment ########
    public void GrabEquipmentItem(int slotIndex)
    {
        // TODO: If an item grabbed from inventory then equip it. If not then it is an equipment slot, so grab the item from there

        if (CharacterInventory.Instance._grabbedInventoryItemSlotIndex != -1)
        {
            // Equip the item logic
            InventorySlot item = CharacterInventory.Instance._inventory[CharacterInventory.Instance._grabbedInventoryItemSlotIndex];
            _EquipItemDrag(CharacterInventory.Instance._grabbedInventoryItemSlotIndex, slotIndex, item);
        }
        else
        {
            // Grab the item logic
            if (_grabbedEquipmentItemSlotIndex == slotIndex)
            {
                // If the grabbed item is the same as the one being grabbed, drop it
                _grabbedEquipmentItemSlotIndex = -1;
                CharacterInventory.Instance._grabbedInventoryItemSlotIndex = -1;
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
        if (!CharacterInventory.Instance._inventory.ContainsKey(slotIndex))
            return;

        InventorySlot InventorySlot = CharacterInventory.Instance._inventory[slotIndex];

        // Check if the item is an equipable item or consumable or not
        if (InventorySlot.Item.type == ItemType.Armor || InventorySlot.Item.type == ItemType.Weapon)
        {
            _QuickItemAction(slotIndex, _GetAvailableEquipmentSlot(InventorySlot), InventorySlot);
        }
        else if (InventorySlot.Item.type == ItemType.Potion || InventorySlot.Item.type == ItemType.Food)
        {
            Character.Instance.ConsumeItem(InventorySlot.Item);
            CharacterInventory.Instance.RemoveInventoryItem(slotIndex);
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

        CharacterInventory.Instance._inventory.Remove(fromEquipSlotIndex);
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
        CharacterInventory.Instance._grabbedInventoryItemSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowEquipmentItemDetails(toEquipSlotIndex);

        CharacterInventory.Instance._inventory.Remove(fromEquipSlotIndex);
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

        CharacterInventory.Instance._inventory.Add(CharacterInventory.Instance._GetNextSlotIndex(), new InventorySlot()
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

    public void _DropGrabbedEquipmentItem(int fromSlotIndex, int toSlotIndex, InventorySlot item)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemUnequip });

        // Update the index of the grabbed item
        _equipments.Remove(fromSlotIndex);
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, this, EventArgs.Empty);
        // Check if clicked inventory slot is empty. If not then find first avaliable slot
        var avaliableIndex = CharacterInventory.Instance._inventory.ContainsKey(toSlotIndex) ? CharacterInventory.Instance._GetNextSlotIndex() : toSlotIndex;
        CharacterInventory.Instance._inventory.Add(avaliableIndex, item);

        CharacterInventory.Instance._grabbedInventoryItemSlotIndex = -1;
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
}