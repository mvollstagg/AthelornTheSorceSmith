using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
using UnityEngine.UI;

public class NPCTest : MonoBehaviour, IInteractable
{
    [SerializeField] private CursorType _cursorType;
    public void HideUI()
    {
        Debug.Log("This NPC is hiding UI", this.gameObject);
    }

    public void Interact()
    {
        Debug.Log("This NPC is interacting", this.gameObject);
    }

    public void ShowUI()
    {
        Debug.Log("This NPC is showing UI", this.gameObject);
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
