using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public Transform _equipmentsGrid;
    [SerializeField] private Transform _itemDetailsPanel;
    [SerializeField] private TextMeshProUGUI _inventoryWeightText;
    [SerializeField] private RectTransform _outlineGlow;
    [SerializeField] private Transform _previewCamera;
    [SerializeField] public RectTransform _grabbedItemSlot;
    [SerializeField] private Canvas _canvas;
    [HideInInspector] public int ItemGridCount => _itemsGrid.childCount;
    [HideInInspector] public int HoveredItemSlot => _outlineGlow.parent.GetSiblingIndex();
    private float _inventoryTotalWeight = 0f;

    
    private void LateUpdate()
    {
        if (InventoryManager.Instance.GrabbedInventoryItemSlotIndex != -1 || InventoryManager.Instance.GrabbedEquipmentItemSlotIndex != -1)
        {
            var _mousePosition = Input.mousePosition;
            var _newPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, _mousePosition, _canvas.worldCamera, out _newPosition);
            _grabbedItemSlot.position = _canvas.transform.TransformPoint(_newPosition + new Vector2(0, -100f));
        }
    }

    public EquipmentSlotType GetEquipmentSlotType(int slotIndex)
    {
        return _equipmentsGrid.GetChild(slotIndex).GetComponent<EquipmentSlotManager>()._slotType;
    }

    public void SetGrabbedItemSlotStatus(bool status)
    {
        _grabbedItemSlot.gameObject.SetActive(status);
    }
    
    public void OnInventoryDisabled()
    {
        _grabbedItemSlot.gameObject.SetActive(false);
        _outlineGlow.gameObject.SetActive(false);
        _previewCamera.gameObject.SetActive(false);
        if (InventoryManager.Instance.GrabbedInventoryItemSlotIndex != -1)
        {
            SetInventoryGridItem(InventoryManager.Instance.GrabbedInventoryItemSlotIndex);
        }
        if (InventoryManager.Instance.GrabbedEquipmentItemSlotIndex != -1)
        {
            SetEquipmentGridItem(InventoryManager.Instance.GrabbedEquipmentItemSlotIndex);
        }
    }

    public void OnInventoryEnabled()
    {
        UpdateEquipmentGrids();
        UpdateInventoryGrids();
        _outlineGlow.gameObject.SetActive(true);
        _previewCamera.gameObject.SetActive(true);
    }

    public void SetInventorySlotIconOpacity(int slotIndex, float opacity)
    {
        _itemsGrid
        .GetChild(slotIndex)
        .Find("Icon")
        .GetComponent<Image>()
        .color = new Color(1f, 1f, 1f, opacity);
    }

    public void SetEquipmentSlotIconOpacity(int slotIndex, float opacity)
    {
        _equipmentsGrid
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

    public void OnInventoryItemHovered(int slotIndex)
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
            ShowInventoryItemDetails(slotIndex);
        }
    }

    public void OnEquipmentItemHovered(int slotIndex)
    {
        _outlineGlow.transform.SetParent(_equipmentsGrid.GetChild(slotIndex));
        _outlineGlow.anchoredPosition = Vector2.zero;
        _outlineGlow.gameObject.SetActive(true);
        
        if (!InventoryManager.Instance.EquipmentItems.ContainsKey(slotIndex))
        {
            _itemDetailsPanel.gameObject.SetActive(false);
        }
        else
        {
            ShowEquipmentItemDetails(slotIndex);
        }
    }

    public void ShowInventoryItemDetails(int slotIndex)
    {
        _itemDetailsPanel.gameObject.SetActive(false);
        if (!InventoryManager.Instance.InventoryItems.TryGetValue(slotIndex, out InventorySlot inventorySlot)) return;
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

    public void ShowEquipmentItemDetails(int slotIndex)
    {
        _itemDetailsPanel.gameObject.SetActive(false);
        if (!InventoryManager.Instance.EquipmentItems.TryGetValue(slotIndex, out InventorySlot inventorySlot)) return;
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

    public void UpdateInventoryGrids()
    {
        // Get the occupied slot indexes
        HashSet<int> occupiedIndexes = new HashSet<int>(InventoryManager.Instance.InventoryItems.Keys);

        // Get the empty slot indexes
        List<int> emptySlotIndexes = Enumerable.Range(0, _itemsGrid.childCount)
            .Except(occupiedIndexes)
            .ToList();

        // Set grid items for occupied slots
        foreach (int occupiedIndex in occupiedIndexes)
        {
            SetInventoryGridItem(occupiedIndex);
        }

        // Unset empty slots
        foreach (int emptySlotIndex in emptySlotIndexes)
        {
            UnsetInventoryGridItem(emptySlotIndex);
        }
    }

    public void UpdateEquipmentGrids()
    {
        // Get the occupied slot indexes
        HashSet<int> occupiedIndexes = new HashSet<int>(InventoryManager.Instance.EquipmentItems.Keys);

        // Get the empty slot indexes
        List<int> emptySlotIndexes = Enumerable.Range(0, _equipmentsGrid.childCount)
            .Except(occupiedIndexes)
            .ToList();

        // Set grid items for occupied slots
        foreach (int occupiedIndex in occupiedIndexes)
        {
            SetEquipmentGridItem(occupiedIndex);
        }

        // Unset empty slots
        foreach (int emptySlotIndex in emptySlotIndexes)
        {
            UnsetEquipmentGridItem(emptySlotIndex);
        }
    }

    private void SetInventoryGridItem(int slotIndex)
    {
        InventorySlot slot = InventoryManager.Instance.InventoryItems[slotIndex];
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);
        slotTransform.Find("Icon").GetComponent<Image>().sprite = slot.Item.icon;
        slotTransform.Find("Icon").gameObject.SetActive(true);
        if (InventoryManager.Instance.GrabbedInventoryItemSlotIndex != slotIndex)
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

    private void SetEquipmentGridItem(int slotIndex)
    {
        InventorySlot slot = InventoryManager.Instance.EquipmentItems[slotIndex];
        Transform slotTransform = _equipmentsGrid.GetChild(slotIndex);
        slotTransform.Find("Background").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        slotTransform.Find("Icon").GetComponent<Image>().sprite = slot.Item.icon;
        slotTransform.Find("Icon").gameObject.SetActive(true);
        if (InventoryManager.Instance.GrabbedEquipmentItemSlotIndex != slotIndex)
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
    }

    public void SetGrabItemSlot(InventorySlot slot)
    {
        var slotTransform = _grabbedItemSlot.gameObject.transform;
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
    }

    private void UnsetInventoryGridItem(int slotIndex)
    {
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);

        slotTransform.Find("Icon").gameObject.SetActive(false);
        slotTransform.Find("Rarity").gameObject.SetActive(false);
        slotTransform.Find("Amount").gameObject.SetActive(false);
    }

    private void UnsetEquipmentGridItem(int slotIndex)
    {
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);

        slotTransform = _equipmentsGrid.GetChild(slotIndex);
        slotTransform.Find("Background").gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        slotTransform.Find("Icon").gameObject.SetActive(false);
        slotTransform.Find("Rarity").gameObject.SetActive(false);
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
}