using System;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
using System.Linq;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class InputManager : Singleton<InputManager>
{
	[Header("Action Maps")]
	[SerializeField] private Dictionary<string, InputActionMap> _inputActionMaps = new Dictionary<string, InputActionMap>();
    public CharacterAssets InputActions;
	public List<string> ActiveActionMaps = new List<string>();
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
	public PlayerInput _playerInput;
	public static bool UsingController => Gamepad.current != null;
	[HideInInspector]
    public bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }
	
	void Start()
	{
		_playerInput = GetComponent<PlayerInput>();
		InputType = _playerInput.currentControlScheme == "KeyboardMouse" ? InputTypeEnum.KeyboardMouse : InputTypeEnum.Gamepad;

		foreach (InputActionMap actionMap in _playerInput.actions.actionMaps)
        {
			// Assign all action maps to the dictionary with their names and disable them
			_inputActionMaps[actionMap.name] = actionMap;
			actionMap.Disable();
        }

		// Enable all action maps needed at first
		_playerInput.actions.FindActionMap("Character").Enable();
		_playerInput.actions.FindActionMap("Transition").Enable();
		_playerInput.actions.FindActionMap("Camera").Enable();

		#region Camera Rotate Actions
		InputActions = new CharacterAssets();
		InputActions.Camera.Enable();
		
		InputActions.Camera.RotateMouse.performed += (obj) => EventManager.Instance.Trigger(GameEvents.ON_CAMERA_ROTATE_MOUSE, obj, EventArgs.Empty);
		InputActions.Camera.RotateMouse.canceled += (obj) => EventManager.Instance.Trigger(GameEvents.ON_CAMERA_ROTATE_MOUSE_CANCELED, obj, EventArgs.Empty);
		InputActions.Camera.RotateGamepad.performed += (obj) => EventManager.Instance.Trigger(GameEvents.ON_CAMERA_ROTATE_GAMEPAD, obj, EventArgs.Empty);
		InputActions.Camera.RotateGamepad.canceled += (obj) => EventManager.Instance.Trigger(GameEvents.ON_CAMERA_ROTATE_GAMEPAD_CANCELED, obj, EventArgs.Empty);

		#endregion
	}

	void Update()
	{
		ActiveActionMaps = _inputActionMaps.Where(x => x.Value.enabled).Select(x => x.Key).ToList();
	}

	public void SwitchActionMap(string actionMapName)
	{
		// Disable all action maps
		foreach (InputActionMap actionMap in _inputActionMaps.Values.Where(x => x.enabled && x.name != ActionMaps.TRANSITION))
		{
			actionMap.Disable();
		}

		// Enable the action map with the given name
		_inputActionMaps[actionMapName].Enable();
	}

	public void EnableActionMap(string actionMapName)
	{
		Instance._inputActionMaps[actionMapName].Enable();
	}

	public void DisableActionMap(string actionMapName)
	{
		Instance._inputActionMaps[actionMapName].Disable();
	}

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

	#region Player Input Actions

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

	public void OnToggleMenu(InputValue value)
	{
		InGameMenuManager.Instance.ToggleInGameMenu();
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

	#endregion
}