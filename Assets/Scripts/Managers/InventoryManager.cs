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
    private int _grabbedSlotIndex = -1;
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
    public int GrabbedSlotIndex => _grabbedSlotIndex;

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
        _grabbedSlotIndex = -1;
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
        if (!_inventory.ContainsKey(selectedSlotIndex) || _grabbedSlotIndex != -1)
            return;

        RemoveItem(selectedSlotIndex);
    }

    public void OnDropItemStackAction()
    {
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (!_inventory.ContainsKey(selectedSlotIndex) || _grabbedSlotIndex != -1)
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
        if (_grabbedSlotIndex == slotIndex)
        {
            // If the grabbed item is the same as the one being grabbed, drop it
            _DropGrabbedItem(slotIndex);
        }
        else if (_grabbedSlotIndex != -1)
        {
            // If there is a different item already grabbed, swap them or drop the grabbed item
            if (_equipments.ContainsKey(slotIndex))
            {
                _SwapItems(_grabbedSlotIndex, slotIndex);
            }
            else
            {
                _DropGrabbedItem(slotIndex);
            }
        }
        else if (_equipments.ContainsKey(slotIndex))
        {
            // If there is no item grabbed, grab the item
            _grabbedSlotIndex = slotIndex;
            InventorySlot inventorySlot = _inventory[slotIndex];
            InventoryUIManager.Instance.SetGrabItemSlot(inventorySlot);
            InventoryUIManager.Instance.SetGrabbedItemSlotStatus(true);
            InventoryUIManager.Instance.SetSlotIconOpacity(_grabbedSlotIndex, 0.25f);
        }

        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    public void EquipItemQuick(int slotIndex)
    {
        if (!_inventory.ContainsKey(slotIndex))
            return;

        InventorySlot item = _inventory[slotIndex];
        _EquipItemQuick(slotIndex, _GetAvailableEquipmentSlot(item), item);
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

    private int _GetAvailableEquipmentSlot(InventorySlot item)
    {
        var equipmentSlotTransforms = InventoryUIManager.Instance._equipmentsGrid.GetComponentsInChildren<EquipmentSlotManager>(true);

        int availableSlotIndex = equipmentSlotTransforms
            .Select((equipmentSlot, index) => new { EquipmentSlot = equipmentSlot, Index = index })
            .FirstOrDefault(x => x.EquipmentSlot._slotType == item.Item.equipmentSlotType)?.Index ?? -1;

        return availableSlotIndex;
    }





    // ! ######## Inventory ########
    public void GrabInventoryItem(int slotIndex)
    {
        if (_grabbedSlotIndex == slotIndex)
        {
            // If the grabbed item is the same as the one being grabbed, drop it
            _DropGrabbedItem(slotIndex);
        }
        else if (_grabbedSlotIndex != -1)
        {
            // If there is a different item already grabbed, swap them or drop the grabbed item
            if (_inventory.ContainsKey(slotIndex))
            {
                _SwapItems(_grabbedSlotIndex, slotIndex);
            }
            else
            {
                _DropGrabbedItem(slotIndex);
            }
        }
        else if (_inventory.ContainsKey(slotIndex))
        {
            // If there is no item grabbed, grab the item
            _grabbedSlotIndex = slotIndex;
            InventorySlot inventorySlot = _inventory[slotIndex];
            InventoryUIManager.Instance.SetGrabItemSlot(inventorySlot);
            InventoryUIManager.Instance.SetGrabbedItemSlotStatus(true);
            InventoryUIManager.Instance.SetSlotIconOpacity(_grabbedSlotIndex, 0.25f);
        }

        InventoryUIManager.Instance.UpdateInventoryGrids();
    }

    private void _SwapItems(int grabbedSlotIndex, int slotIndex)
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


        _grabbedSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.ShowInventoryItemDetails(slotIndex);
    }

    private void _DropGrabbedItem(int slotIndex)
    {
        InventorySlot grabbedItem = _inventory[_grabbedSlotIndex];
        
        // Update the index of the grabbed item
        _inventory.Remove(_grabbedSlotIndex);
        _inventory.Add(slotIndex, grabbedItem);

        _grabbedSlotIndex = -1;
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