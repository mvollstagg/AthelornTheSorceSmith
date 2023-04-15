using System;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;

public class CursorInteractor : MonoBehaviour, IInteractable
{    
    [SerializeField] private CursorType _cursorType;

    private void Start()
    {
        IInteractable interactable = this;
    }

    private void OnMouseEnter()
    {
        EventManager.Instance.Trigger(GameEvents.ON_MOUSE_ENTER_INTERACTABLE, this, new OnMouseInteractableEventArgs { CursorType = _cursorType });
    }

    private void OnMouseExit()
    {
        EventManager.Instance.Trigger(GameEvents.ON_MOUSE_ENTER_INTERACTABLE, this, new OnMouseInteractableEventArgs { CursorType = CursorType.Pointer });
    }
}