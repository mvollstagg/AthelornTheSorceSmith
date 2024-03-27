using Scripts.Entities.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    private Camera cam; // Reference to the main camera
    public float TurnSpeed = 5.2f;

    // Assuming you have these as part of your class now
    private float _remappedMoveInputX = 0f;
    private float _remappedMoveInputY = 0f;

    private void Awake()
    {
        cam = Camera.main; // Ensure we have a reference to the main camera
    }

    void Update()
    {
        // Rotate to face the mouse cursor
        //RotatePlayer();

        // Recalculate the input values based on current direction and input
        RemapInputValues();

        // Update the animator with the recalculated input values
        animator.SetFloat("RemappedMoveInputX", _remappedMoveInputX);
        animator.SetFloat("RemappedMoveInputY", _remappedMoveInputY);
        animator.SetBool("Moving", _remappedMoveInputX != 0f || _remappedMoveInputY != 0f);
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

    private void RotatePlayer()
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);
            Vector3 direction = (worldPosition - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
}
