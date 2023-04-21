using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : Singleton<Character>
{
    #region Public Variables
    [HideInInspector]
    public Camera _mainCamera;
    [HideInInspector]
    public CharacterController _controller;
    public PlayerInput _playerInput;
    public CharacterAssets InputActions;
    public int inventoryMaxWeight = 300;
    #endregion

    #region Close Public Variables
    #endregion

    #region Private Variables
    [HideInInspector]
    public bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }
    #endregion
    
    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _controller = GetComponent<CharacterController>();
    }
}
