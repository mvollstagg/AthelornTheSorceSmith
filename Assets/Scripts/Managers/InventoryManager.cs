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
    [Header("UI References")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _itemsGrid;
    [SerializeField] private Transform _itemDetailsPanel;
    [SerializeField] private TextMeshProUGUI _inventoryWeightText;
    [SerializeField] private RectTransform _outlineGlow;

    [Header("Drag and Drop")]
    [SerializeField] private RectTransform _grabbedItemSlot;
    [SerializeField] private Canvas _canvas;
    [HideInInspector]public UnityEvent<int> itemHover;
    [HideInInspector]public UnityEvent<int> itemGrab;
    private int _hoveredSlotIndex = -1;
    private int _grabbedSlotIndex = -1;
    private float _inventoryTotalWeight = 0f;
    private int _maxNumberOfSlots;
    [SerializeField] public Dictionary<int, InventorySlot> _inventory = new Dictionary<int, InventorySlot>();

    [Header("Test Items")]
    [SerializeField] private InventoryItemDataSO _testItem;
    [SerializeField] private InventoryItemDataSO _testItem2;
    [SerializeField] private InventoryItemDataSO _testItem3;
    [SerializeField] private InventoryItemDataSO _testItem4;
    [SerializeField] private InventoryItemDataSO _testItem5;
    
    [Header("Inventory Slots")]
    public List<int> _inventorySlots = new List<int>();
    

    void Start()
    {
        _maxNumberOfSlots = _itemsGrid.childCount;
        AddItem(_testItem, 1);
        AddItem(_testItem2, 1);
        AddItem(_testItem3, 1);
        AddItem(_testItem4, 1);
        AddItem(_testItem5, 1);

        itemHover = new UnityEvent<int>();
        itemHover.AddListener(_OnItemHovered);
        itemGrab = new UnityEvent<int>();
        itemGrab.AddListener(_OnItemGrabbed);
    }

    protected virtual void LateUpdate()
    {
        var _mousePosition = Input.mousePosition;
        var _newPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, _mousePosition, _canvas.worldCamera, out _newPosition);
        _grabbedItemSlot.position = _canvas.transform.TransformPoint(_newPosition + new Vector2(0, -100f));
    }

    public void ToggleInventoryPanel()
    {
        bool active = !_inventoryPanel.activeSelf;
        if (active) _UpdateGridItems();
        EventSystem.current.SetSelectedGameObject(_itemsGrid.GetChild(0).gameObject);
        _itemsGrid.GetChild(0).gameObject.GetComponent<Selectable>().Select();
        _inventoryPanel.SetActive(active);
    }

    public void AddItem()
    {
        AddItem(_testItem, 20);
        _UpdateGridItems();
    }
        
    public void AddItem(InventoryItemDataSO item, int amount = 1, int dropIndex = -1)
    {
        int remaining = amount;        

        // If there is a slot index to drop the item into, try to add it to that slot
        if (_grabbedSlotIndex == -1)
        {
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
        }        

        // If there are still items remaining to be added, create new slots for them if there is a free slot
        while (remaining > 0)
        {
            // If there are no free slots, exit the function
            if (_inventory.Count >= _maxNumberOfSlots)
                break;

            int stackCount = Mathf.Min(item.maxStackSize, remaining);

            int index = dropIndex == -1 ? _GetNextSlotIndex() : dropIndex;
            _inventory.Add(index, new InventorySlot()
            {
                Item = item,
                Amount = stackCount
            });

            _SetGridItem(index);

            remaining -= stackCount;
        }

        // Update total weight of inventory
        float weightToAdd = item.weight * (amount - remaining);
        _inventoryTotalWeight += weightToAdd;
        _inventoryWeightText.text = $"{_inventoryTotalWeight.ToString("0.0")}/{Character.Instance.inventoryMaxWeight}";

        if (remaining > 0)
        {
            Debug.Log($"{remaining} {item.name} exceeded the maximum stack size.");
        }
    }
    
    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (!_inventory.ContainsKey(slotIndex)) return;

        InventorySlot slot = _inventory[slotIndex];

        if (amount == -1)
            amount = slot.Amount;

        slot.Amount -= amount;

        if (slot.Amount <= 0)
        {
            slot.Amount = 0;
            _UnsetGridItem(slotIndex);
        }
        else
        {
            _SetGridItem(slotIndex);
        }

        _inventoryTotalWeight -= slot.Item.weight * (slot.Amount >= 0 ? amount : slot.Amount);

        if (slot.Amount == 0)
            _inventory.Remove(slotIndex);
        
        _inventoryWeightText.text = $"{_inventoryTotalWeight.ToString("0.0")}/{Character.Instance.inventoryMaxWeight}";
    }

    public void OnDropItemAction()
    {
        int selectedSlotIndex = _outlineGlow.parent.GetSiblingIndex();
        if (!_inventory.ContainsKey(selectedSlotIndex))
            return;

        RemoveItem(selectedSlotIndex);
    }

    public void OnDropItemStackAction()
    {
        int selectedSlotIndex = _outlineGlow.parent.GetSiblingIndex();
        if (!_inventory.ContainsKey(selectedSlotIndex))
            return;

        RemoveItem(selectedSlotIndex, -1);
    }

    public void OnSortInventoryAction()
    {
        List<InventorySlot> sortedSlots = _inventory
            .Values
            .OrderBy((InventorySlot s) => -s.Item.price * s.Amount)
            .ToList();

        Dictionary<int, InventorySlot> newSlots =
            new Dictionary<int, InventorySlot>();
        for (int i = 0; i < sortedSlots.Count; i++)
            newSlots.Add(i, sortedSlots[i]);

        _inventory = newSlots;
        _UpdateGridItems();
        int selectedSlotIndex = _outlineGlow.parent.GetSiblingIndex();
    }

    
    private void _OnItemGrabbed(int slotIndex)
    {
        // Check if there is an item already grabbed
        if (_grabbedSlotIndex != -1)
        {
            // If the grabbed item is the same as the one being grabbed, drop it
            if (_grabbedSlotIndex == slotIndex)
            {
                _DropGrabbedItem(slotIndex);
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
        _SetGridItem(inventorySlot, _grabbedItemSlot);
        _grabbedItemSlot.gameObject.SetActive(true);
        
        _itemsGrid
        .GetChild(_grabbedSlotIndex)
        .Find("Icon")
        .GetComponent<Image>()
        .color = new Color(1f, 1f, 1f, 0.25f);
    }

    private void _SwapItems(int grabbedSlotIndex, int slotIndex)
    {        
        InventorySlot grabbedItem = new()
        {
            Item = _inventory[_grabbedSlotIndex].Item,
            Amount = _inventory[_grabbedSlotIndex].Amount
        };

        InventorySlot swappedItem = new()
        {
            Item = _inventory[slotIndex].Item,
            Amount = _inventory[slotIndex].Amount
        };

        RemoveItem(slotIndex, -1);
        RemoveItem(grabbedSlotIndex, -1);
        
        if(grabbedItem.Item.code == swappedItem.Item.code && grabbedItem.Item.maxStackSize > 1)
        {
            Debug.Log("Same item");
            int totalAmount = grabbedItem.Amount + swappedItem.Amount;
            int stackCount = Mathf.Min(grabbedItem.Item.maxStackSize, totalAmount);
            AddItem(grabbedItem.Item, stackCount, slotIndex);
            AddItem(swappedItem.Item, totalAmount - stackCount, grabbedSlotIndex);
        }
        else
        {
            AddItem(grabbedItem.Item, grabbedItem.Amount, slotIndex);
            AddItem(swappedItem.Item, swappedItem.Amount, grabbedSlotIndex);
        }

        _grabbedSlotIndex = -1;
        _grabbedItemSlot.gameObject.SetActive(false);

        _itemsGrid
        .GetChild(slotIndex)
        .Find("Icon")
        .GetComponent<Image>()
        .color = new Color(1f, 1f, 1f, 1f);
        _itemsGrid
        .GetChild(grabbedSlotIndex)
        .Find("Icon")
        .GetComponent<Image>()
        .color = new Color(1f, 1f, 1f, 1f);
    }

    private void _DropGrabbedItem(int slotIndex)
    {
        InventorySlot droppedItem = new()
        {
            Item = _inventory[_grabbedSlotIndex].Item,
            Amount = _inventory[_grabbedSlotIndex].Amount
        };

        RemoveItem(_grabbedSlotIndex, -1);
        AddItem(droppedItem.Item, droppedItem.Amount, slotIndex);

        _grabbedSlotIndex = -1;
        _grabbedItemSlot.gameObject.SetActive(false);
        
        _UpdateItemDetails(slotIndex);
        _itemsGrid
        .GetChild(slotIndex)
        .Find("Icon")
        .GetComponent<Image>()
        .color = new Color(1f, 1f, 1f, 1f);
    }

    private void _OnItemHovered(int slotIndex)
    {
        _hoveredSlotIndex = slotIndex;
        _outlineGlow.transform.SetParent(_itemsGrid.GetChild(slotIndex));
        _outlineGlow.anchoredPosition = Vector2.zero;
        _outlineGlow.gameObject.SetActive(true);
        
        if (!_inventory.ContainsKey(slotIndex))
        {
            _itemDetailsPanel.gameObject.SetActive(false);
        }
        else
        {
            _UpdateItemDetails(slotIndex);
        }
    }

    private void _UpdateItemDetails(int slotIndex)
    {
        InventorySlot inventorySlot = _inventory[slotIndex];
        _itemDetailsPanel.Find("ItemPreview").GetComponent<Image>().sprite = inventorySlot.Item.icon;
        _itemDetailsPanel.Find("Title").GetComponent<TextMeshProUGUI>().color = ItemRarityColors.GetColor(inventorySlot.Item.rarity);
        _itemDetailsPanel.Find("Title").GetComponent<TextMeshProUGUI>().text = inventorySlot.Item.itemName;
        _itemDetailsPanel.Find("Description").GetComponent<TextMeshProUGUI>().text = inventorySlot.Item.description;
        _itemDetailsPanel.Find("Details").GetComponent<TextMeshProUGUI>().text = inventorySlot.Item.GetDetailsDisplay();
        _itemDetailsPanel.Find("Traits").GetComponent<TextMeshProUGUI>().text = inventorySlot.Item.GetTraitsDisplay();
        _itemDetailsPanel.Find("Types").GetComponent<TextMeshProUGUI>().text = inventorySlot.Item.GetTypesDisplay();
        _itemDetailsPanel.Find("Weight").GetComponent<TextMeshProUGUI>().text = (inventorySlot.Item.weight * inventorySlot.Amount).ToString();
        _itemDetailsPanel.Find("Price").GetComponent<TextMeshProUGUI>().text = inventorySlot.Item.price.ToString();
        _itemDetailsPanel.gameObject.SetActive(true);
    }

    private void _UpdateGridItems()
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
            _SetGridItem(p.Key);

            // Update the current slot index
            currentSlotIndex = p.Key + 1;
        }

        // Clear any remaining grid items
        for (int i = currentSlotIndex; i < _itemsGrid.childCount; i++)
            _UnsetGridItem(i);
    }

    private void _SetGridItem(int slotIndex)
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

    private void _SetGridItem(InventorySlot slot, Transform slotTransform)
    {
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

    private List<int> _DistributeItems(int maxStackSize, int amount)
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

    private void _UnsetGridItem(int slotIndex)
    {
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);
        slotTransform.Find("Icon").gameObject.SetActive(false);
        slotTransform.Find("Rarity").gameObject.SetActive(false);
        slotTransform.Find("Amount").gameObject.SetActive(false);
    }
}
