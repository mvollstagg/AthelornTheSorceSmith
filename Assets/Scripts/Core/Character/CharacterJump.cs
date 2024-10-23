using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CharacterJump : Singleton<CharacterJump>
{
    #region Public Variables
    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    [Header("Free Fall")]
    public bool FreeFall;
    #endregion

    #region Close Public Variables
    #endregion

    #region Private Variables
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    public float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _targetRotation;
    private CharacterController _controller;
    #endregion
    
    void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _controller = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        JumpAndGravity();

		// Check if the player is jumping
		if (!Character.Instance.GetAbility<CharacterGroundCheckAbility>().Grounded || InputManager.Instance.jump)
		{
			if (InputManager.Instance.move != Vector2.zero)
			{
				_targetRotation = Mathf.Atan2(InputManager.Instance.move.x, InputManager.Instance.move.y) * Mathf.Rad2Deg +
								  Camera.main.transform.eulerAngles.y;
			}
			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
			_controller.Move(targetDirection.normalized * (Character.Instance.GetAbility<CharacterMovementAbility>().GetSpeed() * Time.deltaTime) +
							 new Vector3(0.0f, CharacterJump.Instance._verticalVelocity, 0.0f) * Time.deltaTime);
		}
	}

    private void JumpAndGravity()
    {
        if (Character.Instance.GetAbility<CharacterGroundCheckAbility>().Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;
            // ! Check
            FreeFall = false;
            

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (InputManager.Instance.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // ! Check
                FreeFall = true;

            }

            // if we are not grounded, do not jump
            InputManager.Instance.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
}
