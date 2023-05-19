using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler
{
    public int _slotIndex;
    private RectTransform rectTransform;
    private const bool IS_EQUIPMENT = false;

    private void Awake()
    {
        _slotIndex = transform.GetSiblingIndex();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryManager.Instance.itemHover.Invoke(_slotIndex, IS_EQUIPMENT);
    }

    public void OnSelect(BaseEventData eventData)
    {
        InventoryManager.Instance.itemHover.Invoke(_slotIndex, IS_EQUIPMENT);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryManager.Instance.itemGrab.Invoke(_slotIndex, IS_EQUIPMENT);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // InventoryManager.Instance._EquipItem(_slotIndex);
        }
    }
}