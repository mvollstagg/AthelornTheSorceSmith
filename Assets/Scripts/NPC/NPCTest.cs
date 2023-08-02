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
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        HideUI();
    }

    public void HideUI()
    {
        canvas.enabled = false;
    }

    public void Interact()
    {
        Debug.Log("This NPC is interacting", this.gameObject);
    }

    public void ShowUI()
    {
        canvas.enabled = true;
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
