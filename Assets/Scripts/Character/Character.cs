using Scripts.Core;
using UnityEngine;

public class Character : Singleton<Character>
{
    #region Public Variables
    [HideInInspector]
    public Camera _mainCamera;
    [HideInInspector]
    public CharacterController _controller;
    
    public int inventoryMaxWeight = 300;
    #endregion

    #region Close Public Variables
    [SerializeField] private LevelDataSO _levelData;
    #endregion

    #region Private Variables
    
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
