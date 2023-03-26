using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
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
    public bool cursorLocked = false;

    [SerializeField] private Transform cameraFollowTarget;
    private GameInput _input;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    [SerializeField] private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;
    private bool _rotate = false;
    private Vector3 _cursorPosition;

    void Awake()
    {
        _input = GameInput.Instance;
        CinemachineCameraTarget.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = cameraFollowTarget;
		_playerInputActions = new();
        _playerInputActions.Camera.Enable();

		_playerInputActions.Camera.Rotate.performed += OnPlayerInputActions_RotatePerformed;
        _playerInputActions.Camera.Rotate.canceled += OnPlayerInputActions_RotateCanceled;
    }

    private void OnPlayerInputActions_RotatePerformed(InputAction.CallbackContext context)
    {
        _cursorPosition = Mouse.current.position.ReadValue();
        SetCursorStateLocked(true);
        _rotate = true;
    }

    private void OnPlayerInputActions_RotateCanceled(InputAction.CallbackContext context)
    {
        _rotate = false;
        SetCursorStateLocked(false);
        Mouse.current.WarpCursorPosition(_cursorPosition);
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
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

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
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        cursorLocked = newState;
    }
}