using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootUIManager : Singleton<LootUIManager>
{
    [Header("UI References")]
    [SerializeField] private Transform _itemsGrid;
    [SerializeField] private RectTransform _outlineGlow;
    [SerializeField] private Transform _lootBoxPanel;
    [SerializeField] private Transform _itemDetailsPanel;
    [SerializeField] public Dictionary<int, InventorySlot> _slotItems = new Dictionary<int, InventorySlot>();
    [SerializeField] private TextMeshProUGUI _lootBagMoneyText;

    public void ShowLootBag(LootBag lootBag)
    {
        // set slot items by lootbag items
        _slotItems = lootBag.GetItems()
            .Select((item, index) => new
            { 
                Index = index, 
                Item = item
            })
            .ToDictionary(pair => pair.Index, pair => pair.Item);

        InputManager.Instance.SwitchActionMap(ActionMaps.UI);
        UpdateLootGrids();

        // Set money amount
        _lootBagMoneyText.text = "Coins: " + lootBag.GetMoneyAmount().ToString();

        // Show the loot bag
        _lootBoxPanel.gameObject.SetActive(true);
    }

    public void HideLootBag()
    {
        InputManager.Instance.DisableActionMap(ActionMaps.UI);
        InputManager.Instance.EnableActionMap(ActionMaps.CHARACTER);
        InputManager.Instance.EnableActionMap(ActionMaps.CAMERA);
        _lootBoxPanel.gameObject.SetActive(false);
    }

    public void OnLootItemHovered(int slotIndex)
    {
        _outlineGlow.transform.SetParent(_itemsGrid.GetChild(slotIndex));
        _outlineGlow.anchoredPosition = Vector2.zero;
        _outlineGlow.gameObject.SetActive(true);
        
        if (!_slotItems.ContainsKey(slotIndex))
        {
            _itemDetailsPanel.gameObject.SetActive(false);
        }
        else
        {
            ShowLootItemDetails(slotIndex);
        }
    }

    private void ShowLootItemDetails(int slotIndex)
    {
        _itemDetailsPanel.gameObject.SetActive(false);
        if (!_slotItems.TryGetValue(slotIndex, out InventorySlot inventorySlot)) return;
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

    private void UpdateLootGrids()
    {
        // Get the occupied slot indexes
        HashSet<int> occupiedIndexes = new HashSet<int>(_slotItems.Keys);

        // Get the empty slot indexes
        List<int> emptySlotIndexes = Enumerable.Range(0, _itemsGrid.childCount)
            .Except(occupiedIndexes)
            .ToList();

        // Set grid items for occupied slots
        foreach (int occupiedIndex in occupiedIndexes)
        {
            SetLootGridItem(occupiedIndex);
        }

        // Unset empty slots
        foreach (int emptySlotIndex in emptySlotIndexes)
        {
            UnsetLootGridItem(emptySlotIndex);
        }
    }

    private void SetLootGridItem(int slotIndex)
    {
        InventorySlot slot = _slotItems[slotIndex];
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

    private void UnsetLootGridItem(int slotIndex)
    {
        Transform slotTransform = _itemsGrid.GetChild(slotIndex);

        slotTransform.Find("Icon").gameObject.SetActive(false);
        slotTransform.Find("Rarity").gameObject.SetActive(false);
        slotTransform.Find("Amount").gameObject.SetActive(false);
    }
}
