using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class LootBag : MonoBehaviour, IInteractable
{
    [SerializeField] private CursorType _cursorType;
    private int maxSlots = 6;  // Maximum number of item slots in the bag
    public int money;  // Amount of money in the bag

    [SerializeField] private List<InventorySlot> items = new List<InventorySlot>();  // List of items in the bag
    private float timer = 20f; // 2 minutes
    private Canvas canvas;
    private TextMeshProUGUI timerText;
    private bool isShowingUI = false;

    private void Start()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        canvas = GetComponentInChildren<Canvas>();
        HideUI();
    }

    private void LateUpdate()
    {
        if (isShowingUI)
            timerText.text = string.Format("{0}:{1:00}", Mathf.Floor(timer / 60), timer % 60);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            CharacterInteraction.Instance.RemoveInteractableFromNearbyList(this);
            LootManager.Instance.ResetLootBag(this);
            Destroy(this.gameObject);
        }
    }

    // Add an item to the bag
    public void AddItem(InventorySlot item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.LogWarning("The loot bag is already full!");
            return;
        }

        items.Add(item);
    }

    // Remove an item from the bag
    public void RemoveItem(InventorySlot item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    // Retrieve all items in the bag
    public List<InventorySlot> GetItems()
    {
        return items;
    }

    // Check if the bag is full
    public bool IsFull()
    {
        return items.Count >= maxSlots;
    }

    // Retrieve the amount of money in the bag
    public int GetMoneyAmount()
    {
        return money;
    }

    // Set the amount of money in the bag
    public void SetMoneyAmount(int amount)
    {
        money = amount;
    }

    public void ShowUI()
    {
        isShowingUI = true;
        canvas.enabled = true;
    }

    public void HideUI()
    {
        isShowingUI = false;
        canvas.enabled = false;
    }

    public void Interact()
    {
        LootManager.Instance.InteractLootBag(this);
    }

    public void OnMouseEnter()
    {
        EventManager.Instance.Trigger(GameEvents.ON_MOUSE_ENTER_INTERACTABLE, this, new OnMouseInteractableEventArgs { CursorType = _cursorType });
    }

    public void OnMouseExit()
    {
        EventManager.Instance.Trigger(GameEvents.ON_MOUSE_ENTER_INTERACTABLE, this, new OnMouseInteractableEventArgs { CursorType = CursorType.Pointer });
    }
}