using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;

public class CharacterOrientation : Singleton<CharacterOrientation>
{
    #region Public Variables
    [Tooltip("Sprint speed of the character in m/s")]
    public float TurnSpeed = 15.2f;
    #endregion

    #region Close Public Variables
    #endregion

    #region Private Variables
    private float _rotationVelocity;
    private float _remappedMoveInputX;
    private float _remappedMoveInputY;
    private float _lastRotation;
    private Camera cam; // Reference to the main camera
    #endregion

    private void Awake()
    {
        cam = Camera.main; // Ensure we have a reference to the main camera
    }

    void Update()
    {
        RemapInputValues();
        RotatePlayer();
    }
    
    private void RotatePlayer()
    {
        if (CharacterAnimator.Instance.LocomotionMode == LocomotionModeType.Combat)
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 targetPosition = ray.GetPoint(position);
                Quaternion targetRotation = Quaternion.LookRotation(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
                transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (TurnSpeed * Time.deltaTime) * TurnSpeed);
            }

        }
        else
        {
            // Rotate based on movement direction
            if (InputManager.Instance.move != Vector2.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(InputManager.Instance.move.x, 0f, InputManager.Instance.move.y)), Time.deltaTime * TurnSpeed);
            }
        }
    }

    private void RemapInputValues()
    {
        Vector3 inputDirection = new Vector3(InputManager.Instance.move.x, 0.0f, InputManager.Instance.move.y).normalized;

        if (inputDirection != Vector3.zero)
        {
            // Calculate the angle between the character's forward direction and the input direction
            float angle = Vector3.SignedAngle(transform.forward, inputDirection, Vector3.up);

            // Calculate the remapped values for X and Y based on the angle
            float remappedX = Mathf.Clamp(Mathf.Sin(Mathf.Deg2Rad * angle), -0.5f, 0.5f) * 2.0f;
            float remappedY = Mathf.Clamp(Mathf.Cos(Mathf.Deg2Rad * angle), -0.5f, 0.5f) * 2.0f;

            // Assign the remapped values to your private variables
            _remappedMoveInputX = remappedX;
            _remappedMoveInputY = remappedY;
        }
        else
        {
            // If there is no input, reset the remapped values to zero
            _remappedMoveInputX = 0.0f;
            _remappedMoveInputY = 0.0f;
        }
    }


    public Vector2 GetInputDirection()
    {
        return new Vector2(_remappedMoveInputX, _remappedMoveInputY);
    }
}
