using UnityEngine;
using UnityEngine.EventSystems;
using Scripts.Entities.Enum;

public class EquipmentSlotManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler
{
    public EquipmentSlotType _slotType;
    private int _slotIndex;
    private RectTransform rectTransform;
    private const bool IS_EQUIPMENT = true;

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
            InventoryManager.Instance._UnequipItem(_slotIndex);
        }
    }
}