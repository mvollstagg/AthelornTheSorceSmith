using Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInteraction", menuName = "Character Abilities/Interaction")]
public class CharacterInteractionAbility : ScriptableObject, ICharacterAbility
{
    public float interactionRadius = 3f;

    private SphereCollider interactionCollider;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable closestInteractable;

    private Transform transform;

    public void Initialize(GameObject character)
    {
        // Add a SphereCollider component to create the interaction radius
        interactionCollider = character.AddComponent<SphereCollider>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactionRadius;
        transform = character.transform;
    }

    public void UpdateAbility()
    {
        ScanInteraction();
    }

    void ScanInteraction()
    {
        // Check if the closest interactable is still in the nearby interactables list
        if (closestInteractable != null && !nearbyInteractables.Contains(closestInteractable))
        {
            closestInteractable.HideUI();
            closestInteractable = null;
        }

        // Check if the closest interactable is still the closest interactable
        if (closestInteractable != null)
        {
            float distance = Vector3.Distance(transform.position, closestInteractable.transform.position);
            if (distance > interactionRadius)
            {
                closestInteractable.HideUI();
                closestInteractable = null;
            }
        }

        // Find the closest interactable object among the nearby interactables
        if (closestInteractable == null)
        {
            float closestDistance = Mathf.Infinity;
            Vector3 characterPosition = transform.position;

            foreach (IInteractable interactable in nearbyInteractables)
            {
                // Check if the interactable object is destroyed before accessing it
                if (interactable == null)
                {
                    nearbyInteractables.Remove(interactable);
                    continue;
                }

                float distance = Vector3.Distance(characterPosition, interactable.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        // Show the UI for the closest interactable and hide the UI for the rest
        foreach (IInteractable interactable in nearbyInteractables)
        {
            if (interactable == closestInteractable)
                interactable.ShowUI();
            else
                interactable.HideUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
            UpdateClosestInteractable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Remove(interactable);
            interactable.HideUI();
            UpdateClosestInteractable();
        }
    }

    private void UpdateClosestInteractable()
    {
        closestInteractable = null;
        float closestDistance = Mathf.Infinity;
        Vector3 characterPosition = transform.position;

        // Find the closest interactable object among the nearby interactables
        foreach (IInteractable interactable in nearbyInteractables)
        {
            // Check if the interactable object is destroyed before accessing it
            if (interactable == null)
            {
                nearbyInteractables.Remove(interactable);
                continue;
            }

            float distance = Vector3.Distance(characterPosition, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        // Show the UI for the closest interactable and hide the UI for the rest
        foreach (IInteractable interactable in nearbyInteractables)
        {
            bool isClosest = interactable == closestInteractable;
            if (isClosest)
            {
                interactable.ShowUI();
            }
            else
            {
                interactable.HideUI();
            }
        }
    }

    public void Interact()
    {
        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }

    public void RemoveInteractableFromNearbyList(IInteractable interactable)
    {
        if (closestInteractable == interactable)
        {
            closestInteractable = null;
        }
        nearbyInteractables.Remove(interactable);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the interaction radius in the Unity Editor Scene view
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f); // Set the color with transparency
        Gizmos.DrawSphere(transform.position, interactionRadius);

        // Update the SphereCollider radius to match the interactionRadius
        if (interactionCollider != null)
        {
            interactionCollider.radius = interactionRadius;
        }
    }
}
