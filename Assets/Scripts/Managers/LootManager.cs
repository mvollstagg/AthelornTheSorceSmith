using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    public Transform LootBagTransform;

    public void DropItem(InventoryItemDataSO item)
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
}
