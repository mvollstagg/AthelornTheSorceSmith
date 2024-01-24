using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using UnityEngine;

public class CharacterInventory : Singleton<CharacterInventory>
{
    [Header("Drag and Drop")]
    public int _grabbedInventoryItemSlotIndex = -1;

    [Header("Test Items")]
    [SerializeField] private List<InventoryItemDataSO> _testItems;

    [Header("Active Slots")]
    [SerializeField] public List<InventoryDetails> _inventorySlots = new List<InventoryDetails>();

    [Header("Inventory Object")]
    [SerializeField] public Dictionary<int, InventorySlot> _inventory = new Dictionary<int, InventorySlot>();

    public Dictionary<int, InventorySlot> InventoryItems => _inventory;

    public int GrabbedInventoryItemSlotIndex => _grabbedInventoryItemSlotIndex;

    private void Start()
    {
        _testItems.ForEach(x => AddItem(x, 1));
    }

    private void LateUpdate()
    {
        _inventorySlots = _inventory.Keys.Select(x => new InventoryDetails { Index = x, Name = _inventory[x].Item.name }).ToList();
        _equipmentSlots = _equipments.Keys.Select(x => new InventoryDetails { Index = x, Name = _equipments[x].Item.name }).ToList();
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
    }

        public void AddItem()
    {
       AddItem(_testItems[Random.Range(0, _testItems.Count)], 1);
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

        InventoryUIManager.Instance.UpdateInventoryGrids();
    }
}
