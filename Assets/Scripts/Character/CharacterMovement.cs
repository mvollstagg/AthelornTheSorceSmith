using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

public class CharacterMovement : Singleton<CharacterMovement>
{
    #region Public Variables
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    #endregion

    #region Close Public Variables
    #endregion

    #region Private Variables
    public float _inputMagnitude;
    private float _speed;
    public float _animationBlend;
    public float _maxAnimationBlend;
    private float _targetRotation = 0.0f;
    #endregion

    private void Start()
    {
        _maxAnimationBlend = SprintSpeed;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // Determine if the character is sprinting based on input.
        bool isSprinting = InputManager.Instance.sprint;

        // Calculate the target speed.
        float targetSpeed = isSprinting ? SprintSpeed : MoveSpeed;

        // If there's no movement input, set the target speed to 0.
        if (InputManager.Instance.move == Vector2.zero) targetSpeed = 0.0f;

        // Use input magnitude as a simple proxy for "current speed" for the purpose of lerping.
        // This assumes a direct correlation between input magnitude and intended speed.
        float inputMagnitude = InputManager.Instance.move.magnitude;

        // Simplify the speed calculation. Since Root Motion handles actual movement,
        // this speed value is used more as a state indicator for the animation blend.
        _speed = Mathf.Lerp(_speed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

        // Calculate animation blend value. This should align with your animation blend tree,
        // where the blend value dictates the transition between animations.
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

        // Ensure the blend value doesn't drop below a minimal threshold.
        if (_animationBlend < 0.01f) _animationBlend = 0f;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public float GetInputMagnitude()
    {
        return _inputMagnitude;
    }

    public float GetAnimationBlendMagnitude()
    {
        return _speed / MoveSpeed;
    }
}
