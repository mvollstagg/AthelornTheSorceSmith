using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
using UnityEngine.EventSystems;

public class LootSlotManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler
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
        LootUIManager.Instance.OnLootItemHovered(_slotIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        EventManager.Instance.Trigger(GameEvents.ON_PLAY_SFX, this, new OnSoundEffectsPlayEventArgs { SoundEffectsType = SoundEffectsType.ItemHover });
        LootUIManager.Instance.OnLootItemHovered(_slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LootManager.Instance.TakeLootItem(_slotIndex);
        // if (eventData.button == PointerEventData.InputButton.Left)
        // {
        // }
        // else if (eventData.button == PointerEventData.InputButton.Right)
        // {
        //     // if (LootManager.Instance.GrabbedLootItemSlotIndex != -1 || LootManager.Instance.GrabbedEquipmentItemSlotIndex != -1) return;
        //     // LootManager.Instance.QuickItemAction(_slotIndex);
        // }
    }
}