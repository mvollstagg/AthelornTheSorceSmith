using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
    [Header("UI References")]
    [SerializeField] private Transform _itemsGrid;
    [SerializeField] private Transform _equipmentsGrid;
    [SerializeField] private Transform _itemDetailsPanel;
    [SerializeField] private TextMeshProUGUI _inventoryWeightText;
    [SerializeField] private RectTransform _outlineGlow;
    [SerializeField] private Transform _previewCamera;
    [SerializeField] public RectTransform _grabbedItemSlot;
    [SerializeField] private Canvas _canvas;
    [HideInInspector] public int ItemGridCount => _itemsGrid.childCount;
    [HideInInspector] public int HoveredItemSlot => _outlineGlow.parent.GetSiblingIndex();
    

    private float _inventoryTotalWeight = 0f;

    public void SetGrabbedItemSlotStatus(bool status)
    {
        _grabbedItemSlot.gameObject.SetActive(status);
    }

    
    private void LateUpdate()
    {
        if (InventoryManager.Instance.GrabbedSlotIndex != -1)
        {
            var _mousePosition = Input.mousePosition;
            var _newPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, _mousePosition, _canvas.worldCamera, out _newPosition);
            _grabbedItemSlot.position = _canvas.transform.TransformPoint(_newPosition + new Vector2(0, -100f));
        }
    }

    public void OnInventoryDisabled()
    {
        _grabbedItemSlot.gameObject.SetActive(false);
        _outlineGlow.gameObject.SetActive(false);
        _previewCamera.gameObject.SetActive(false);
        SetGridItem(InventoryManager.Instance.GrabbedSlotIndex);
    }

    public void OnInventoryEnabled()
    {
        _outlineGlow.gameObject.SetActive(true);
        _previewCamera.gameObject.SetActive(true);
    }

    public void SetSlotIconOpacity(int slotIndex, float opacity)
    {
        _itemsGrid
        .GetChild(slotIndex)
        .Find("Icon")
        .GetComponent<Image>()
        .color = new Color(1f, 1f, 1f, opacity);
    }

    public void SetInventoryWeight(float weightToAdd)
    {
        _inventoryTotalWeight += weightToAdd;
        _inventoryWeightText.text = $"{_inventoryTotalWeight.ToString("0.0")}/{Character.Instance.inventoryMaxWeight}";
    }

    public void OnItemHovered(int slotIndex)
    {
        _outlineGlow.transform.SetParent(_itemsGrid.GetChild(slotIndex));
        _outlineGlow.anchoredPosition = Vector2.zero;
        _outlineGlow.gameObject.SetActive(true);
        
        if (!InventoryManager.Instance.InventoryItems.ContainsKey(slotIndex))
        {
            _itemDetailsPanel.gameObject.SetActive(false);
        }
        else
        {
            UpdateItemDetails(slotIndex);
        }
    }

    public void UpdateItemDetails(int slotIndex)
    {
        InventorySlot inventorySlot = InventoryManager.Instance.InventoryItems[slotIndex];
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

    public void UpdateGridItems()
    {
        // Keep track of the current slot index
        int currentSlotIndex = 0;

        // Iterate through each slot in the inventory dictionary
        foreach (KeyValuePair<int, InventorySlot> p in InventoryManager.Instance.InventoryItems)
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

    public void SetGridItem(int slotIndex)
    {
        InventorySlot slot = InventoryManager.Instance.InventoryItems[slotIndex];
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);
        slotTransform.Find("Icon").GetComponent<Image>().sprite = slot.Item.icon;
        slotTransform.Find("Icon").gameObject.SetActive(true);
        slotTransform.Find("Icon").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
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

    public void SetGridItem(InventorySlot slot, Transform slotTransform)
    {
        slotTransform.Find("Icon").GetComponent<Image>().sprite = slot.Item.icon;
        slotTransform.Find("Icon").gameObject.SetActive(true);
        slotTransform.Find("Icon").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        if (slot.Item.rarity != ItemRarity.Common)
        {
            slotTransform.Find("Rarity").GetComponent<Image>().color = ItemRarityColors.GetColor(slot.Item.rarity);
            slotTransform.Find("Rarity").gameObject.SetActive(true);
        }
        else
        {
            slotTransform.Find("Rarity").gameObject.SetActive(false);
        }

        if (slot.Amount > 1)
        {
            slotTransform.Find("Amount").GetComponent<TextMeshProUGUI>().text = slot.Amount.ToString();
            slotTransform.Find("Amount").gameObject.SetActive(true);
        }
        else
        {
            slotTransform.Find("Amount").gameObject.SetActive(false);
        }

        // if (isEquipmentSlot)
        // {
        //     slotTransform.Find("Background").gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        // }
    }

    public List<int> DistributeItems(int maxStackSize, int amount)
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

    public void UnsetGridItem(int slotIndex)
    {
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);

        // if (isEquipmentSlot)
        // {
        //     slotTransform = _equipmentsGrid.GetChild(slotIndex);
        //     slotTransform.Find("Background").gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        //     slotTransform.Find("Icon").gameObject.SetActive(false);
        //     slotTransform.Find("Rarity").gameObject.SetActive(false);
        //     return;
        // }

        slotTransform.Find("Icon").gameObject.SetActive(false);
        slotTransform.Find("Rarity").gameObject.SetActive(false);
        slotTransform.Find("Amount").gameObject.SetActive(false);
    }
}
