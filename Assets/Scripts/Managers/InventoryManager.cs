using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Drag and Drop")]
    private int _hoveredSlotIndex = -1;
    public int _grabbedInventoryItemSlotIndex = -1;
    public int _grabbedEquipmentItemSlotIndex = -1;
    private int _maxNumberOfSlots;
    [SerializeField] public Dictionary<int, InventorySlot> _inventory = new Dictionary<int, InventorySlot>();
    [SerializeField] public Dictionary<int, InventorySlot> _equipments = new Dictionary<int, InventorySlot>();

    [Header("Test Items")]
    [SerializeField] private List<InventoryItemDataSO> _testItems;

    [Header("Active Slots")]
    [SerializeField] public List<InventoryDetails> _inventorySlots = new List<InventoryDetails>();
    [SerializeField] public List<InventoryDetails> _equipmentSlots = new List<InventoryDetails>();
    
    public Dictionary<int, InventorySlot> InventoryItems => _inventory;
    public Dictionary<int, InventorySlot> EquipmentItems => _equipments;      
    public int GrabbedInventoryItemSlotIndex => _grabbedInventoryItemSlotIndex;
    public int GrabbedEquipmentItemSlotIndex => _grabbedEquipmentItemSlotIndex;

    private void Start()
    {
        _maxNumberOfSlots = InventoryUIManager.Instance.ItemGridCount;
        _testItems.ForEach(x => AddItem(x, 1));
    }

    private void LateUpdate()
    {
        _inventorySlots = _inventory.Keys.Select(x => new InventoryDetails { Index = x, Name = _inventory[x].Item.name }).ToList();
        _equipmentSlots = _equipments.Keys.Select(x => new InventoryDetails { Index = x, Name = _equipments[x].Item.name }).ToList();
    }
    
    public void AddItem()
    {
        AddItem(_testItems[Random.Range(0, _testItems.Count)], 1);
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    public void OnInventoryDisabled()
    {
        InventoryUIManager.Instance.OnInventoryDisabled();
        _hoveredSlotIndex = -1;
        _grabbedInventoryItemSlotIndex = -1;
        _grabbedEquipmentItemSlotIndex = -1;
    }

    public void OnInventoryEnabled()
    {
        InventoryUIManager.Instance.OnInventoryEnabled();
    }

    // ! ######## Common ########

    public void AddItem(InventoryItemDataSO item, int amount = 1)
    {
        int remaining = amount;

        // Look for existing slots that can hold more items if item max stack size is greater than 1
        if (item.maxStackSize > 1)
        {
            foreach (var slot in _inventory.Where(x => x.Value.Item.code == item.code))
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
            var existingSlot = _inventory.FirstOrDefault(slot => slot.Value.Item.code == item.code && slot.Value.Amount < slot.Value.Item.maxStackSize);

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
                if (_inventory.Count >= _maxNumberOfSlots)
                    break;

                int stackCount = Mathf.Min(item.maxStackSize, remaining);
                
                _inventory.Add(_GetNextSlotIndex(), new InventorySlot()
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

    public void RemoveItem(int slotIndex, int amount = 1)
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

    
    // ! ######## Input Methods ########
    public void OnDropItemAction()
    {
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (!_inventory.ContainsKey(selectedSlotIndex) || _grabbedInventoryItemSlotIndex != -1)
            return;

        RemoveItem(selectedSlotIndex);
    }

    public void OnDropItemStackAction()
    {
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (!_inventory.ContainsKey(selectedSlotIndex) || _grabbedInventoryItemSlotIndex != -1)
            return;

        RemoveItem(selectedSlotIndex, -1);
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
                _grabbedInventoryItemSlotIndex = -1;
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
                        _equipments.Add(slotIndex, _equipments[_grabbedEquipmentItemSlotIndex]);
                        _equipments.Remove(_grabbedEquipmentItemSlotIndex);
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
                // If there is no item grabbed, grab the item
                _grabbedEquipmentItemSlotIndex = slotIndex;
                InventorySlot inventorySlot = _equipments[slotIndex];
                InventoryUIManager.Instance.SetGrabItemSlot(inventorySlot);
                InventoryUIManager.Instance.SetGrabbedItemSlotStatus(true);
                // InventoryUIManager.Instance.SetEquipmentSlotIconOpacity(_grabbedInventoryItemSlotIndex, 0.25f);
            }
        }

        InventoryUIManager.Instance.UpdateInventoryGrids();
        InventoryUIManager.Instance.UpdateEquipmentGrids();
    }

    // TODO: Rename it to QuickItemAction
    public void EquipItemQuick(int slotIndex)
    {
        if (!_inventory.ContainsKey(slotIndex))
            return;

        InventorySlot InventorySlot = _inventory[slotIndex];

        // Check if the item is an equipable item or consumable or not
        if (InventorySlot.Item.type == ItemType.Armor || InventorySlot.Item.type == ItemType.Weapon)
        {
            _EquipItemQuick(slotIndex, _GetAvailableEquipmentSlot(InventorySlot), InventorySlot);
        }
        else if (InventorySlot.Item.type == ItemType.Potion || InventorySlot.Item.type == ItemType.Food)
        {
            Debug.Log("Consumable item");
        }
        else return;
    }

    private void _EquipItemQuick(int fromEquipSlotIndex, int toEquipSlotIndex, InventorySlot item)
    {
        // Check if the slot is empty. if not empty, unequip the item first before equipping the new one
        if (_equipments.ContainsKey(toEquipSlotIndex))
            _UnequipItem(toEquipSlotIndex);
            
        _equipments.Add(toEquipSlotIndex, new InventorySlot()
        {
            Item = item.Item,
            Amount = 1
        });
        
        _inventory.Remove(fromEquipSlotIndex);
        InventoryUIManager.Instance.UpdateEquipmentGrids();
        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    private void _EquipItemDrag(int fromEquipSlotIndex, int toEquipSlotIndex, InventorySlot item)
    {
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
        _inventory.Add(_GetNextSlotIndex(), new InventorySlot()
        {
            Item = _equipments[slotIndex].Item,
            Amount = 1
        });
        _equipments.Remove(slotIndex);
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
        // Update the index of the grabbed item
        _equipments.Remove(fromSlotIndex);
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
}