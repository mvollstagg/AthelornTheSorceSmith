using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarriorAnims;

public class RootMotionController : Singleton<RootMotionController>
{
    public Animator animator; // Reference to the Animator component
    private Camera cam; // Reference to the main camera
    public float TurnSpeed = 5.2f;
    public float rotationSpeed = 10.0f; // Adjust this value as needed for desired rotation speed

    // Assuming you have these as part of your class now
    private float _remappedMoveInputX = 0f;
    private float _remappedMoveInputY = 0f;

    public LocomotionModeType LocomotionMode { get; set; } = LocomotionModeType.Idle;

    private void Awake()
    {
        cam = Camera.main; // Ensure we have a reference to the main camera
        EventManager.Instance.AddListener<OnCharacterLocomotionChangedEventArgs>(GameEvents.ON_CHARACTER_LOCOMOTION_MODE_CHANGED, OnCharacterLocomotionModeChanged);
    }

    private void OnCharacterLocomotionModeChanged(object sender, OnCharacterLocomotionChangedEventArgs e)
    {
        LocomotionMode = e.LocomotionMode;
        animator.SetInteger(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionMode.GetValue<LocomotionModeType, int>());
    }

    void Update()
    {
        // Rotate to face the mouse cursor
        RotatePlayer();

        // Recalculate the input values based on current direction and input
        RemapInputValues();

        // Jump
        Jump();

        // Update the animator with the recalculated input values
        animator.SetFloat("RemappedMoveInputX", _remappedMoveInputX);
        animator.SetFloat("RemappedMoveInputY", _remappedMoveInputY);
        animator.SetBool("Moving", _remappedMoveInputX != 0f || _remappedMoveInputY != 0f);
    }

    private void Jump()
    {
        if (InputManager.Instance.jump)
        {
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
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
            float remappedX = Mathf.Clamp(Mathf.Sin(Mathf.Deg2Rad * angle), -0.5f, 0.5f) * 1.0f;
            float remappedY = Mathf.Clamp(Mathf.Cos(Mathf.Deg2Rad * angle), -0.5f, 0.5f) * 1.0f;

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

    private void RotatePlayer()
    {
        if (LocomotionMode == LocomotionModeType.Combat)
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 targetPosition = ray.GetPoint(position);
                Quaternion targetRotation = Quaternion.LookRotation(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
                transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
            }

        }
        else
        {
            // Rotate based on movement direction
            if (InputManager.Instance.move != Vector2.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(InputManager.Instance.move.x, 0f, InputManager.Instance.move.y)), Time.deltaTime * rotationSpeed);
            }
        }
    }
}
