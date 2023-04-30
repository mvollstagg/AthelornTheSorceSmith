using System.Diagnostics;
using System;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class InputManager : Singleton<InputManager>
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	public InputTypeEnum InputType;
	private PlayerInput _playerInput;
	public static bool UsingController => Gamepad.current != null;
	
	void Start()
	{
		_playerInput = GetComponent<PlayerInput>();
		InputType = _playerInput.currentControlScheme == "KeyboardMouse" ? InputTypeEnum.KeyboardMouse : InputTypeEnum.Gamepad;
		_playerInput.actions.FindActionMap("InGameMenu").Enable();
	}

	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if(cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}

	public void OnInventory(InputValue value)
	{
		InventoryManager.Instance.ToggleInventoryPanel();
	}

	public void OnAddItem(InputValue value)
	{
		InventoryManager.Instance.AddItem();
	}

	public void OnControlsChanged(PlayerInput input)
	{
		InputType = input.currentControlScheme == "KeyboardMouse" ? InputTypeEnum.KeyboardMouse : InputTypeEnum.Gamepad;
	}

	public void OnSwitch(InputValue value)
	{
		// TODO: Change current control scheme
		if(InputType == InputTypeEnum.KeyboardMouse)
		{
			_playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
		}
		else if(InputType == InputTypeEnum.Gamepad)
		{
			_playerInput.SwitchCurrentControlScheme("KeyboardMouse", Keyboard.current, Mouse.current);
		}
	}

	public void OnDropItem(InputValue value)
	{
		InventoryManager.Instance.OnDropItemAction();
	}

	public void OnDropItemStack(InputValue value)
	{
		InventoryManager.Instance.OnDropItemStackAction();
	}

	public void OnSortInventory(InputValue value)
	{
		InventoryManager.Instance.OnSortInventoryAction();
	}

	public void OnItemSelect(InputValue value)
	{
		// InventoryManager.Instance.itemGrab.Invoke();
	}

	// ####################################################################################################


	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	} 

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}