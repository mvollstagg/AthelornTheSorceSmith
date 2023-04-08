using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

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
    #endregion
    
    void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    
    void Update()
    {
        JumpAndGravity();
    }

    private void JumpAndGravity()
    {
        if (CharacterGroundCheck.Instance.Grounded)
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
