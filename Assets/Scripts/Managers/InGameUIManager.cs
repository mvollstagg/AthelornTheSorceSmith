using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : Singleton<InGameUIManager>
{
    [Header("UI References")]
    [SerializeField] private Transform lootBagUI;

    public void ShowLootBagUI(LootBag lootBag)
    {
        Debug.Log("Showing loot bag UI");
        // lootBagUI.gameObject.SetActive(true);

        // // Clear the UI
        // foreach (Transform child in lootBagUI)
        // {
        //     Destroy(child.gameObject);
        // }

        // // Display the money amount
        // if (lootBag.GetMoneyAmount() > 0)
        // {
        //     GameObject moneyUI = Instantiate(Resources.Load<GameObject>("UI/MoneyUI"), lootBagUI);
        //     moneyUI.GetComponentInChildren<TextMeshProUGUI>().text = lootBag.GetMoneyAmount().ToString();
        // }

        // // Display the items
        // foreach (InventoryItemDataSO item in lootBag.GetItems())
        // {
        //     GameObject itemUI = Instantiate(Resources.Load<GameObject>("UI/ItemUI"), lootBagUI);
        //     itemUI.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
        // }

        lootBag.GetItems().ForEach(item => Debug.Log("this item is shown in UI: " + item.itemName));
    }
}