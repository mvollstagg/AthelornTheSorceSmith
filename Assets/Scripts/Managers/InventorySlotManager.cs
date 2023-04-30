using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler
{
    public int _slotIndex;
    private RectTransform rectTransform;

    private void Awake()
    {
        _slotIndex = transform.GetSiblingIndex();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryManager.Instance.itemHover.Invoke(_slotIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        InventoryManager.Instance.itemHover.Invoke(_slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryManager.Instance.itemGrab.Invoke(_slotIndex);
    }
}