using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class ExpPoint : MonoBehaviour, IInteractable
{
    public int expAmount = 100;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        HideUI();
    }

    public void ShowUI()
    {
        canvas.enabled = true;
    }

    public void HideUI()
    {
        canvas.enabled = false;
    }

    public void Interact()
    {
        Character.Instance.GainExperience(this.expAmount);
        CharacterInteraction.Instance.RemoveInteractableFromNearbyList(this);
        Destroy(this.gameObject);
    }

    public void OnMouseEnter()
    {
    }

    public void OnMouseExit()
    {
    }
}