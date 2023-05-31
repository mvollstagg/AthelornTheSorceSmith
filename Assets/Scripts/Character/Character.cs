using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using AthelornTheSorceSmith.Assets.Scripts.Core;
using Scripts.Entities.Enum;

public class Character : Singleton<Character>, ICharacter
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

    public void ConsumeItem(InventoryItemDataSO item)
    {
        if (item.type == ItemType.Potion || item.type == ItemType.Food)
        {
            foreach (var trait in item.traits)
            {
                var traitStatusMultiplier = trait.Status == TraitStatus.Positive ? 1 : -1;
                if (trait.Type == TraitType.Health)
                {
                    health += trait.Value * traitStatusMultiplier;
                    health = Mathf.Clamp(health, 0, _characterData.maxHealth);
                }
                else if (trait.Type == TraitType.Mana)
                {
                    mana += trait.Value * traitStatusMultiplier;
                    mana = Mathf.Clamp(mana, 0, _characterData.maxMana);
                }
                else if (trait.Type == TraitType.Stamina)
                {
                    stamina += trait.Value * traitStatusMultiplier;
                    stamina = Mathf.Clamp(stamina, 0, _characterData.maxStamina);
                }
            }
        }
    }
    
    private void Awake()
    {
        health = _characterData.maxHealth / 2;
        mana = _characterData.maxMana / 2;
        stamina = _characterData.maxStamina / 2;

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
