using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private int _slotIndex;

    private void Awake()
    {
        _slotIndex = transform.GetSiblingIndex();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryManager.Instance.itemSelected.Invoke(_slotIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        InventoryManager.Instance.itemSelected.Invoke(_slotIndex);
    }
}