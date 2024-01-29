using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;

public class CharacterOrientation : Singleton<CharacterOrientation>
{
    #region Public Variables
    [Tooltip("Sprint speed of the character in m/s")]
    public float TurnSpeed = 5.2f;
    public TMP_Text _debugText;
    #endregion

    #region Close Public Variables
    #endregion

    #region Private Variables
    private float _rotationVelocity;
    private float _remappedMoveInputX;
    private float _remappedMoveInputY;
    private float _lastRotation;
    #endregion

    void Update()
    {
        _debugText.text = $"Input X: {_remappedMoveInputX}<br><br>Input Y: {_remappedMoveInputY}";
        RemapInputValues();

        if (InputManager.Instance.IsCurrentDeviceMouse)
        {
            RotatePlayer();
        }
    }
    
    private void RotatePlayer()
    {
        if(CharacterAnimator.Instance.LocomotionMode == LocomotionModeType.Combat)
        {
            // Raycast from the camera to the point on the ground where the mouse is pointing
            Ray ray = Character.Instance._mainCamera.ScreenPointToRay(CursorManager.Instance._mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Calculate the angle between the character and the point on the ground
                Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                Vector3 direction = targetPosition - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                float angle = rotation.eulerAngles.y;
                _lastRotation = angle;

                // Rotate the character towards the point on the ground, only rotating around the y-axis
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, angle, 0f), TurnSpeed * Time.deltaTime);
            }
        }
        else
        {
            float rotation = _lastRotation;
            Vector3 inputDirection = new Vector3(InputManager.Instance.move.x, 0.0f, InputManager.Instance.move.y).normalized;

            if (inputDirection != Vector3.zero)
            {
                var _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      Character.Instance._mainCamera.transform.eulerAngles.y;
                rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    CharacterMovement.Instance.RotationSmoothTime);
                
                _lastRotation = rotation;
            }

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, _lastRotation, 0.0f);
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
