using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
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
    public bool cursorLocked = true;

    [SerializeField] private Transform cameraFollowTarget;
    private InputManager _inputManager;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    // private CharacterInputs _characterInputs;
    private bool _rotate = false;
    private bool _cursedPositionSet;
    private Vector3 _cursorPosition;    

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
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}