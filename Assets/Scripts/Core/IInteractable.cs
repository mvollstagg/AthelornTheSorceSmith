using System;
using UnityEngine;

namespace Scripts.Core
{
    public interface IInteractable
    {
        Transform transform { get; }
        void Interact();
        void ShowUI();
        void HideUI();
        void OnMouseEnter();
        void OnMouseExit();
    }
}