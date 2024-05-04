using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    public Transform LootBagTransform;
    private LootBag _lootBag;

    public void DropItem(InventorySlot item)
    {
        // Calculate the throw position and direction
        Vector3 throwPosition = Character.Instance.transform.position + Character.Instance.transform.forward * 2f;
        Quaternion throwRotation = Character.Instance.transform.rotation;

        // Randomize the throw position within a range
        float randomOffsetX = Random.Range(-0.5f, 0.5f);
        float randomOffsetZ = Random.Range(-0.5f, 0.5f);
        throwPosition += new Vector3(randomOffsetX, 0f, randomOffsetZ);

        // Instantiate the loot bag at the calculated position and rotation
        var lootBagObject = Instantiate(LootBagTransform, throwPosition, throwRotation);
        LootBag lootBag = lootBagObject.GetComponent<LootBag>();

        // Add the dropped item to the loot bag
        lootBag.AddItem(item);

        // Set the money amount in the loot bag (if applicable)
        lootBag.SetMoneyAmount(0);  // Set the initial money amount
    }

    public void DropLoot(List<InventorySlot> items, int moneyAmount)
    {
        // Calculate the throw position and direction
        Vector3 throwPosition = Character.Instance.transform.position + Character.Instance.transform.forward * 2f;
        Quaternion throwRotation = Character.Instance.transform.rotation;

        // Randomize the throw position within a range
        float randomOffsetX = Random.Range(-0.5f, 0.5f);
        float randomOffsetZ = Random.Range(-0.5f, 0.5f);
        throwPosition += new Vector3(randomOffsetX, 0f, randomOffsetZ);

        // Instantiate the loot bag at the calculated position and rotation
        var lootBagObject = Instantiate(LootBagTransform, throwPosition, throwRotation);
        LootBag lootBag = lootBagObject.GetComponent<LootBag>();

        // Add the dropped items to the loot bag
        items.ForEach(item => lootBag.AddItem(item));

        // Set the money amount in the loot bag (if applicable)
        lootBag.SetMoneyAmount(moneyAmount);
    }

    public void TakeLootItems()
    {
        // Check if the loot bag destroyed before the player takes the items
        if (_lootBag == null)
        {
            LootUIManager.Instance.HideLootBag();
            return;
        }

        // Add the items from the loot bag to the player's inventory
        _lootBag.GetItems()
                .ForEach(item => InventoryManager.Instance.AddItem(item.Item, item.Amount));

        // Add the money from the loot bag to the player's inventory
        InventoryManager.Instance.AddMoney(_lootBag.GetMoneyAmount());

        // Remove the loot bag from the scene
        CharacterInteraction.Instance.RemoveInteractableFromNearbyList(_lootBag);
        Destroy(_lootBag.gameObject);
        LootUIManager.Instance.HideLootBag();
    }

    public void TakeLootItem(int slotIndex)
    {
        // Check if the loot bag destroyed before the player takes the items
        if (_lootBag == null)
        {
            LootUIManager.Instance.HideLootBag();
            return;
        }

        if (slotIndex < 0 || slotIndex >= _lootBag.GetItems().Count) return;

        var item = _lootBag.GetItems()[slotIndex];

        // Add the item from the loot bag to the player's inventory
        InventoryManager.Instance.AddItem(item.Item, item.Amount);

        // Remove the item from the loot bag
        _lootBag.RemoveItem(item);

        // If the loot bag is empty, remove it from the scene
        if (_lootBag.GetItems().Count == 0)
        {
            CharacterInteraction.Instance.RemoveInteractableFromNearbyList(_lootBag);
            Destroy(_lootBag.gameObject);
            LootUIManager.Instance.HideLootBag();
        }
    }

    public void InteractLootBag(LootBag lootBag)
    {
        _lootBag = lootBag;
        LootUIManager.Instance.ShowLootBag(_lootBag);
    }

    public void ResetLootBag(LootBag lootBag)
    {
        if (_lootBag == lootBag)
            _lootBag = null;
    }
}
