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
        InventoryUIManager.Instance.OnEquipmentItemHovered(_slotIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        InventoryUIManager.Instance.OnEquipmentItemHovered(_slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryManager.Instance.GrabEquipmentItem(_slotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventoryManager.Instance.GrabbedInventoryItemSlotIndex != -1 || InventoryManager.Instance.GrabbedEquipmentItemSlotIndex != -1) return;
            InventoryManager.Instance.UnequipItemQuick(_slotIndex);
        }
    }
}