using Scripts.Entities.Class;
using Scripts.Entities.Enum;
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
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemHover });
        InventoryUIManager.Instance.OnInventoryItemHovered(_slotIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemHover });
        InventoryUIManager.Instance.OnInventoryItemHovered(_slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryManager.Instance.GrabInventoryItem(_slotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventoryManager.Instance.GrabbedInventoryItemSlotIndex != -1 || InventoryManager.Instance.GrabbedEquipmentItemSlotIndex != -1) return;
            InventoryManager.Instance.EquipItemQuick(_slotIndex);
        }
    }
}