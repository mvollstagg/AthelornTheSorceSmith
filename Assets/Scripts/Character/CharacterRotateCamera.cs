using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Enum;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRotateCamera : Singleton<CharacterRotateCamera>
{
    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 55.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = 45.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;

    [SerializeField] private Transform cameraFollowTarget;
    private InputManager _inputManager;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.1f;
    private bool _rotate = false;
    private bool _cursedPositionSet;
    private Vector3 _cursorPosition;
    private InputTypeEnum _inputType;

    void Awake()
    {
        _inputManager = InputManager.Instance;
        CinemachineCameraTarget.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = cameraFollowTarget;
        _inputType = InputManager.Instance.InputType;
        Character.Instance.InputActions = new CharacterAssets();
        Character.Instance.InputActions.Camera.Enable();
        if(_inputType == InputTypeEnum.KeyboardMouse)
        {
            Character.Instance.InputActions.Camera.RotateMouse.performed += OnPlayerInputActions_RotateMousePerformed;
            Character.Instance.InputActions.Camera.RotateMouse.canceled += OnPlayerInputActions_RotateMouseCanceled;
        }
        else if(_inputType == InputTypeEnum.Gamepad)
        {
            Character.Instance.InputActions.Camera.RotateGamepad.performed += OnPlayerInputActions_RotateGamepadPerformed;
            Character.Instance.InputActions.Camera.RotateGamepad.canceled += OnPlayerInputActions_RotateGamepadCanceled;
        }		
    }

    private void OnPlayerInputActions_RotateGamepadCanceled(InputAction.CallbackContext context)
    {
        _rotate = false;
        Mouse.current.WarpCursorPosition(_cursorPosition);
    }

    private void OnPlayerInputActions_RotateGamepadPerformed(InputAction.CallbackContext context)
    {
        _rotate = true;
        _cursorPosition = Mouse.current.position.ReadValue();
    }

    private void OnPlayerInputActions_RotateMousePerformed(InputAction.CallbackContext context)
    {
        _cursorPosition = Mouse.current.position.ReadValue();
        _rotate = true;
        SetCursorStateLocked(true);
    }

    private void OnPlayerInputActions_RotateMouseCanceled(InputAction.CallbackContext context)
    {
        _rotate = false;
        SetCursorStateLocked(false);
        Mouse.current.WarpCursorPosition(_cursorPosition);
    }
    
    void Update()
    {
        if(_inputType != InputManager.Instance.InputType)
        {
            Debug.Log("Input Type Changed");
            _inputType = InputManager.Instance.InputType;
            if(_inputType == InputTypeEnum.KeyboardMouse)
            {
                Character.Instance.InputActions.Camera.RotateMouse.performed += OnPlayerInputActions_RotateMousePerformed;
                Character.Instance.InputActions.Camera.RotateMouse.canceled += OnPlayerInputActions_RotateMouseCanceled;
            }
            else if(_inputType == InputTypeEnum.Gamepad)
            {
                Character.Instance.InputActions.Camera.RotateGamepad.performed += OnPlayerInputActions_RotateGamepadPerformed;
                Character.Instance.InputActions.Camera.RotateGamepad.canceled += OnPlayerInputActions_RotateGamepadCanceled;
            }
        }
    }

    void OnDestroy()
    {
        Character.Instance.InputActions.Camera.Disable();
        Character.Instance.InputActions.Camera.RotateMouse.performed -= OnPlayerInputActions_RotateMousePerformed;
        Character.Instance.InputActions.Camera.RotateMouse.canceled -= OnPlayerInputActions_RotateMouseCanceled;
        Character.Instance.InputActions.Camera.RotateGamepad.performed -= OnPlayerInputActions_RotateGamepadPerformed;
        Character.Instance.InputActions.Camera.RotateGamepad.canceled -= OnPlayerInputActions_RotateGamepadCanceled;
    }

    void LateUpdate()
    {
        if (_rotate)
        {
            CameraRotation();
        }
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_inputManager.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = Character.Instance.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            _cinemachineTargetYaw += _inputManager.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _inputManager.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void SetCursorStateLocked(bool newState)
    {
        if(!Character.Instance.IsCurrentDeviceMouse)
            return;
        cursorLocked = newState;
        // Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }    
}