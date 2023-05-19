using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{

    [Header("Drag and Drop")]
    [HideInInspector] public UnityEvent<int, bool> itemHover = new UnityEvent<int, bool>();
    [HideInInspector] public UnityEvent<int, bool> itemGrab = new UnityEvent<int, bool>();
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
    public int HoveredSlotIndex => _hoveredSlotIndex;

    private void Start()
    {
        _maxNumberOfSlots = InventoryUIManager.Instance.ItemGridCount;

        itemHover.AddListener(_OnItemHovered);
        itemGrab.AddListener(_OnItemGrabbed);

        _testItems.ForEach(x => AddItem(x, 1));
    }

    private void _OnItemHovered(int slotIndex, bool isEquipmentSlot)
    {
        _hoveredSlotIndex = slotIndex;
        InventoryUIManager.Instance.OnItemHovered(slotIndex);
    }

    private void LateUpdate()
    {
        _inventorySlots = _inventory.Keys.Select(x => new InventoryDetails { Index = x, Name = _inventory[x].Item.name }).ToList();
        _equipmentSlots = _equipments.Keys.Select(x => new InventoryDetails { Index = x, Name = _equipments[x].Item.name }).ToList();
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

    public void AddItem()
    {
        AddItem(_testItems[Random.Range(0, _testItems.Count)], 1);
        // AddItem(_testItems[17], 20);
        InventoryUIManager.Instance.UpdateGridItems();
    }
        
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

        InventoryUIManager.Instance.UpdateGridItems();
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
        else
            InventoryUIManager.Instance.UpdateItemDetails(slotIndex);

        // Multiply by -1 to remove weight
        InventoryUIManager.Instance.SetInventoryWeight(slot.Item.weight * (slot.Amount >= 0 ? amount : slot.Amount) * -1);
        InventoryUIManager.Instance.UpdateGridItems();
    }

    public void OnDropItemAction()
    {
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (!_inventory.ContainsKey(selectedSlotIndex))
            return;

        RemoveItem(selectedSlotIndex);
    }

    public void OnDropItemStackAction()
    {
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
        if (!_inventory.ContainsKey(selectedSlotIndex))
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
        InventoryUIManager.Instance.UpdateGridItems();
        int selectedSlotIndex = InventoryUIManager.Instance.HoveredItemSlot;
    }

    // ! ######## Equipment ########
    private void _EquipItem(int slotIndex, InventorySlot item)
    {
        // Check if the slot is empty. if not empty, unequip the item first before equipping the new one
        if (_equipments.ContainsKey(slotIndex))
            _UnequipItem(slotIndex);
            
        _equipments.Add(slotIndex, new InventorySlot()
        {
            Item = item.Item,
            Amount = 1
        });
        // TODO: Find a way to send equipment slot transform to ui or send only the slot index and make the ui find the slot transform
        // InventoryUIManager.Instance.SetGridItem(item, _equipmentsGrid.GetChild(slotIndex), true);
        RemoveItem(_grabbedSlotIndex);
        _grabbedSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.UpdateGridItems();
    }

    public void _UnequipItem(int slotIndex)
    {
        AddItem(_equipments[slotIndex].Item, 1);
        _equipments.Remove(slotIndex);
        InventoryUIManager.Instance.UpdateGridItems();
    }


    // ! ######## Inventory ########
    private void _OnItemGrabbed(int slotIndex, bool isEquipmentSlot = false)
    {
        // Check if there is an item already grabbed
        if (_grabbedSlotIndex != -1)
        {
            // If the grabbed item is the same as the one being grabbed, drop it
            if (_grabbedSlotIndex == slotIndex)
            {
                _grabbedSlotIndex = -1;
                InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
                InventoryUIManager.Instance.UpdateGridItems();
                return;
            }
            // If the grabbed item is different, swap them
            else
            {
                if (_inventory.ContainsKey(slotIndex))
                {
                    _SwapItems(_grabbedSlotIndex, slotIndex);
                }
                else
                {
                    _DropGrabbedItem(slotIndex);
                }
                InventoryUIManager.Instance.UpdateGridItems();
            }
        }
        // If there is no item grabbed, grab the item
        else
        {
            if (_inventory.ContainsKey(slotIndex))
                _GrabItem(slotIndex);
        }

    }

    private void _GrabItem(int slotIndex)
    {
        _grabbedSlotIndex = slotIndex;
        InventorySlot inventorySlot = _inventory[slotIndex];
        // TODO: Chhange SetGridItem grabbedItemSlot later
        InventoryUIManager.Instance.SetGridItem(inventorySlot, InventoryUIManager.Instance._grabbedItemSlot);
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(true);
        InventoryUIManager.Instance.SetSlotIconOpacity(_grabbedSlotIndex, 0.25f);
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
        InventoryUIManager.Instance.UpdateItemDetails(slotIndex);
    }


    private void _DropGrabbedItem(int slotIndex)
    {
        InventorySlot grabbedItem = _inventory[_grabbedSlotIndex];
        
        // Update the index of the grabbed item
        _inventory.Remove(_grabbedSlotIndex);
        _inventory.Add(slotIndex, grabbedItem);

        _grabbedSlotIndex = -1;
        InventoryUIManager.Instance.SetGrabbedItemSlotStatus(false);
        InventoryUIManager.Instance.UpdateItemDetails(slotIndex);
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
