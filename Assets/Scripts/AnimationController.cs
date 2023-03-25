using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    private const string ACTION = "Action";
    private const string TRIGGER = "Trigger";
    private PlayerInputActions playerInputActions;
    [SerializeField] private Animator animator;
    void Awake()
	{
		playerInputActions = new();
        playerInputActions.Interaction.Enable();

		playerInputActions.Interaction.GetItem.performed += PlayerInputActions_GetItem;
		playerInputActions.Interaction.ChopStart.performed += PlayerInputActions_ChopStart;
	}

    private void PlayerInputActions_ChopStart(InputAction.CallbackContext context)
    {
        animator.SetInteger(ACTION, 2);
        animator.SetTrigger(TRIGGER);
    }

    private void PlayerInputActions_GetItem(InputAction.CallbackContext context)
    {
        animator.SetInteger(ACTION, 1);
        animator.SetTrigger(TRIGGER);
    }
}
