using System;
using System.Collections.Generic;
using AthelornTheSorceSmith.Assets.Scripts.Core;
using Scripts.Core;
using Scripts.Entities.Enum;
using UnityEditor;
using UnityEngine;

public class Character : Singleton<Character>, ICharacter
{
    [HideInInspector] public Camera _mainCamera;
    [HideInInspector] public CharacterController _controller;

    [Header("Character Stats")] public int money;
    public int inventoryMaxWeight = 300;
    
    [SerializeField] private CharacterDataSO _characterData;
    [SerializeField] private LevelDataSO _levelData;
    
    public CharacterDataSO GetCharacterData() => _characterData;
    public LevelDataSO GetLevelData() => _levelData;

    public List<ScriptableObject> abilities;

    private List<ICharacterAbility> initializedAbilities = new List<ICharacterAbility>();

    // TODO: This action will return given type of ability. For example GetType<CharacterMovementAbility>() will return CharacterMovementAbility
    public T GetAbility<T>() where T : ScriptableObject
    {
        foreach (var ability in abilities)
        {
            if (ability is T t)
            {
                return t;
            }
        }

        return null;
    }

    private void Awake()
    {
        // Initialize all abilities
        foreach (ScriptableObject ability in abilities)
        {
            if (ability is ICharacterAbility characterAbility)
            {
                characterAbility.Initialize(this.gameObject);
                initializedAbilities.Add(characterAbility);
            }
        }

        // get a reference to our main camera
        if (_mainCamera == null) _mainCamera = Camera.main;
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        foreach (ICharacterAbility ability in initializedAbilities)
        {
            ability.UpdateAbility();
        }
    }


    public void ConsumeItem(InventoryItemDataSO item)
    {
        if (item.type == ItemType.Potion || item.type == ItemType.Food)
        {
            foreach (var trait in item.traits)
            {
                var traitStatusMultiplier = trait.Status == TraitStatus.Positive ? 1 : -1;
                if (trait.Type == TraitType.Health)
                {
                    _characterData.currentHealth += trait.Value * traitStatusMultiplier;
                    _characterData.currentHealth = Mathf.Clamp((float)_characterData.currentHealth, 0,
                        (float)_characterData.maxHealth);
                }
                else if (trait.Type == TraitType.Mana)
                {
                    _characterData.currentMana += trait.Value * traitStatusMultiplier;
                    _characterData.currentMana = Mathf.Clamp((float)_characterData.currentMana, 0,
                        (float)_characterData.maxMana);
                }
                else if (trait.Type == TraitType.Stamina)
                {
                    _characterData.currentStamina += trait.Value * traitStatusMultiplier;
                    _characterData.currentStamina = Mathf.Clamp((float)_characterData.currentStamina, 0,
                        (float)_characterData.maxStamina);
                }
            }

            _characterData.SaveStats();
        }
    }

    public void AddStatPoints(string stat, int points)
    {
        if (_characterData.statPoints >= points)
        {
            _characterData.statPoints -= points;

            switch (stat)
            {
                case "Strength":
                    _characterData.strength += points;
                    break;
                case "Dexterity":
                    _characterData.dexterity += points;
                    break;
                case "Intelligence":
                    _characterData.intelligence += points;
                    break;
                case "Vitality":
                    _characterData.vitality += points;
                    break;
                case "Focus":
                    _characterData.focus += points;
                    break;
                case "Charisma":
                    _characterData.charisma += points;
                    break;
            }
        }
    }

    public void GainExperience(int experience)
    {
        _characterData.GainExperience(experience);
    }

    private void LevelUp()
    {
        // Perform any additional level up actions here
    }

    public int GetRequiredExperienceForLevel(int level)
    {
        return _levelData.GetRequiredExperience(level);
    }
}