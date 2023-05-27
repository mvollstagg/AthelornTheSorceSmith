using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

public class Character : Singleton<Character>
{
    #region Public Variables
    [HideInInspector]
    public Camera _mainCamera;
    [HideInInspector]
    public CharacterController _controller;
    
    [Header("Character Stats")]
    public int inventoryMaxWeight = 300;
    public int health;
    public int mana;
    public int stamina;
    #endregion

    #region Close Public Variables
    [SerializeField] private CharacterDataSO _characterData;
    [SerializeField] private LevelDataSO _levelData;
    #endregion

    #region Private Variables
    
    #endregion
    
    private void Awake()
    {
        health = _characterData.maxHealth;
        mana = _characterData.maxMana;
        stamina = _characterData.maxStamina;

        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _controller = GetComponent<CharacterController>();
    }

    public Dictionary<string, CharacterStat> GetStats()
    {
        var stats = new Dictionary<string, CharacterStat>();
        stats["Health"] = new CharacterStat("Health", health, _characterData.maxHealth);
        stats["Mana"] = new CharacterStat("Mana", mana, _characterData.maxMana);
        stats["Stamina"] = new CharacterStat("Stamina", stamina, _characterData.maxStamina);
        return stats;
    }
}

public class CharacterStat
{
    public string name;
    public int maxValue;
    public int currentValue;

    public CharacterStat(string name, int currentValue, int maxValue)
    {
        this.name = name;
        this.currentValue = currentValue;
        this.maxValue = maxValue;
    }
}
