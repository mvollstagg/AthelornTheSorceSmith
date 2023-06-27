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

    public void ShowUI()
    {
        
    }

    public void HideUI()
    {
        
    }

    public void Interact()
    {
        Character.Instance.GainExperience(this.expAmount);
    }

    public void OnMouseEnter()
    {
    }

    public void OnMouseExit()
    {
    }
}