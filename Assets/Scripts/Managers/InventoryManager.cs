using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("UI References")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _itemsGrid;
    private Dictionary<int, InventorySlot> _inventory = new Dictionary<int, InventorySlot>();
    
    public void ToggleInventoryPanel()
    {
        bool active = !_inventoryPanel.activeSelf;
        if (active) UpdateGridItems();
        _inventoryPanel.SetActive(active);
    }

    // TODO: Add fake items to test 
    [SerializeField] private InventoryItemDataSO _testItem;
    [SerializeField] private InventoryItemDataSO _testItem2;
    [SerializeField] private InventoryItemDataSO _testItem3;
    [SerializeField] private InventoryItemDataSO _testItem4;
    public UnityEvent<int> itemSelected;

    void Start()
    {
        AddItem(_testItem, 990);
        AddItem(_testItem2, 999);
        AddItem(_testItem, 999);
        AddItem(_testItem3, 999);

        itemSelected = new UnityEvent<int>();
        itemSelected.AddListener(_OnItemSelected);
    }

    private void _OnItemSelected(int slotIndex)
    {
        // show item details
        Debug.Log($"Item selected: {slotIndex}" + 
                  $"\nItem: {_inventory[slotIndex].Item.name}" +
                  $"\nAmount: {_inventory[slotIndex].Amount}");
    }

    public void AddItem()
    {
        AddItem(_testItem4, 1);
        UpdateGridItems();
    }

        
    public void AddItem(InventoryItemDataSO item, int amount = 1)
    {
        int remaining = amount;

        // Try to add to existing slots
        foreach (var slot in _inventory.Values)
        {
            // If the slot can't hold more items or it doesn't match the item being added, skip it
            if (slot.Amount == slot.Item.maxStackSize || slot.Item.code != item.code)
                continue;

            // Calculate the amount of items that can be added to the slot without exceeding its max stack size
            int availableToAdd = slot.Item.maxStackSize - slot.Amount;

            // If we can add all remaining items to this slot, do so and exit the function
            if (remaining <= availableToAdd)
            {
                slot.Amount += remaining;
                remaining = 0;
                break;
            }

            // Otherwise, fill the slot to its max stack size and continue to the next slot
            slot.Amount += availableToAdd;
            remaining -= availableToAdd;
        }

        // If there are still items remaining to be added, create new slots for them
        while (remaining > 0)
        {
            int stackCount = Mathf.Min(item.maxStackSize, remaining);

            int index = GetNextSlotIndex();
            _inventory.Add(index, new InventorySlot()
            {
                Item = item,
                Amount = stackCount
            });

            SetGridItem(index);

            remaining -= stackCount;
        }
    }


    private void UpdateGridItems()
    {
        // Keep track of the current slot index
        int currentSlotIndex = 0;

        // Iterate through each slot in the inventory dictionary
        foreach (KeyValuePair<int, InventorySlot> p in _inventory)
        {
            // If the slot index is less than the current slot index, skip it
            if (p.Key < currentSlotIndex)
                continue;

            // Set the grid item for the slot
            SetGridItem(p.Key);

            // Update the current slot index
            currentSlotIndex = p.Key + 1;
        }

        // Clear any remaining grid items
        for (int i = currentSlotIndex; i < _itemsGrid.childCount; i++)
            UnsetGridItem(i);
    }


    private void SetGridItem(int slotIndex)
    {
        InventorySlot slot = _inventory[slotIndex];
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);
        slotTransform.Find("Icon").GetComponent<Image>().sprite = slot.Item.icon;
        slotTransform.Find("Icon").gameObject.SetActive(true);
        if (slot.Amount > 1)
        {
            slotTransform.Find("Amount").GetComponent<TextMeshProUGUI>().text = slot.Amount.ToString();
            slotTransform.Find("Amount").gameObject.SetActive(true);
        }
        else
        {
            slotTransform.Find("Amount").gameObject.SetActive(false);
        }
        if (slot.Item.rarity != ItemRarity.Common)
        {
            slotTransform.Find("Rarity").GetComponent<Image>().color = ItemRarityColors.GetColor(slot.Item.rarity);
            slotTransform.Find("Rarity").gameObject.SetActive(true);
        }
        else
        {
            slotTransform.Find("Rarity").gameObject.SetActive(false);
        }
    }

    private List<int> DistributeItems(int maxStackSize, int amount)
    {
        List<int> stackCounts = new List<int>();

        int nStacks = amount / maxStackSize;
        for (int i = 0; i < nStacks; i++)
            stackCounts.Add(maxStackSize);

        int remaining = amount % maxStackSize;
        if (remaining > 0)
            stackCounts.Add(remaining);

        return stackCounts;
    }

    private int GetNextSlotIndex()
    {
        if (_inventory.Count == 0) return 0;
    
        int index = 0;
        foreach (var pair in _inventory)
        {
            if (pair.Key == index)
            {
                index++;
            }
            else
            {
                break;
            }
        }        
        return index;
    }

    private void UnsetGridItem(int slotIndex)
    {
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);
        slotTransform.Find("Icon").gameObject.SetActive(false);
        slotTransform.Find("Rarity").gameObject.SetActive(false);
        slotTransform.Find("Amount").gameObject.SetActive(false);
    }
}
