using System;
using System.Collections;
using System.Collections.Generic;
using JohnTheBlacksmith.Assets.Scripts.Core;
using Scripts.Core.Singletons;
using UnityEngine;

public class CursorInteractor : MonoBehaviour, IInteractable
{
    public static event EventHandler<OnMouseEnterInteractableInteractableEventArgs> OnMouseEnterInteractable;
    public class OnMouseEnterInteractableInteractableEventArgs : EventArgs
    {
        public CursorType CursorType;
    };

    new public static void ResetStaticData()
    {
        OnMouseEnterInteractable = null;
    }
    
    [SerializeField] private CursorType _cursorType;

    private void Start()
    {
        IInteractable interactable = this;
    }

    private void OnMouseEnter()
    {
        OnMouseEnterInteractable?.Invoke(this, new OnMouseEnterInteractableInteractableEventArgs { CursorType = _cursorType });
    }

    private void OnMouseExit()
    {
        OnMouseEnterInteractable?.Invoke(this, new OnMouseEnterInteractableInteractableEventArgs { CursorType = CursorType.Pointer });
    }
}