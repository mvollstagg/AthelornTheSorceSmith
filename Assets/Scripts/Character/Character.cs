using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using AthelornTheSorceSmith.Assets.Scripts.Core;
using Scripts.Entities.Enum;
using Scripts.Entities.Class;
using UnityEditor;

public class Character : Singleton<Character>, ICharacter
{
    #region Public Variables
    [HideInInspector]
    public Camera _mainCamera;
    [HideInInspector]
    public CharacterController _controller;

    [Header("Character Stats")]
    public int money = 0;
    public int inventoryMaxWeight = 300;
    #endregion

    #region Close Public Variables
    [SerializeField] private CharacterDataSO _characterData;
    [SerializeField] private LevelDataSO _levelData;
    #endregion

    #region Private Variables
    private GUIStyle labelStyle;

    public int RequiredExperienceForNextLevel { get; private set; }
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
                    _characterData.currentHealth += trait.Value * traitStatusMultiplier;
                    _characterData.currentHealth = Mathf.Clamp((float)_characterData.currentHealth, 0, (float)_characterData.maxHealth);
                }
                else if (trait.Type == TraitType.Mana)
                {
                    _characterData.currentMana += trait.Value * traitStatusMultiplier;
                    _characterData.currentMana = Mathf.Clamp((float)_characterData.currentMana, 0, (float)_characterData.maxMana);
                }
                else if (trait.Type == TraitType.Stamina)
                {
                    _characterData.currentStamina += trait.Value * traitStatusMultiplier;
                    _characterData.currentStamina = Mathf.Clamp((float)_characterData.currentStamina, 0, (float)_characterData.maxStamina);
                }
            }
            _characterData.SaveStats();
        }
    }
    
    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _controller = GetComponent<CharacterController>();

        
    }

    public CharacterDataSO GetCharacterData()
    {
        return _characterData;
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

#if UNITY_EDITOR
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Character character = (Character)target;

            EditorGUILayout.Space();

            character.DrawStatsGUI();
        }
    }

    private void InitializeGUIStyles()
    {
        labelStyle = new GUIStyle(EditorStyles.label);
        // TODO: Add custom styles here
    }

    private void DrawStatsGUI()
    {
        InitializeGUIStyles();
        EditorGUILayout.LabelField("Derrived Stats", EditorStyles.boldLabel);
        DrawProgressBar("Health", (int)_characterData.currentHealth, (int)_characterData.maxHealth, Color.red);
        DrawProgressBar("Mana", (int)_characterData.currentMana, (int)_characterData.maxMana, Color.blue);
        DrawProgressBar("Stamina", (int)_characterData.currentStamina, (int)_characterData.maxStamina, new Color(1f, 0.7f, 0f));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Physical Damage: " + _characterData.physicalDamage, labelStyle);
        EditorGUILayout.LabelField("Physical Defence: " + _characterData.physicalDefence, labelStyle);
        EditorGUILayout.LabelField("Magical Damage: " + _characterData.magicalDamage, labelStyle);
        EditorGUILayout.LabelField("Magical Defence: " + _characterData.magicalDefence, labelStyle);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Critical Chance: " + _characterData.criticalStrikeChance + "%", labelStyle);
        EditorGUILayout.LabelField("Critical Damage: " + _characterData.criticalStrikeDamage + "%", labelStyle);
        EditorGUILayout.LabelField("Accuracy: " + _characterData.accuracy, labelStyle);
        EditorGUILayout.LabelField("Dodge Chance: " + _characterData.dodgeChance + "%", labelStyle);
        EditorGUILayout.LabelField("Block Chance: " + _characterData.blockChance + "%", labelStyle);
        EditorGUILayout.LabelField("Influence Bonus: " + _characterData.influenceBonus + "%", labelStyle);
        EditorGUILayout.LabelField("Negotiation Bonus: " + _characterData.negotiationBonus + "%", labelStyle);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Fire Resistance: " + _characterData.fireResistance + "%", labelStyle);
        EditorGUILayout.LabelField("Water Resistance: " + _characterData.waterResistance + "%", labelStyle);
        EditorGUILayout.LabelField("Earth Resistance: " + _characterData.earthResistance + "%", labelStyle);
        EditorGUILayout.LabelField("Air Resistance: " + _characterData.airResistance + "%", labelStyle);
        EditorGUILayout.LabelField("Poison Resistance: " + _characterData.poisonResistance + "%", labelStyle);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Strength: " + _characterData.strength, labelStyle);
        EditorGUILayout.LabelField("Dexterity: " + _characterData.dexterity, labelStyle);
        EditorGUILayout.LabelField("Intelligence: " + _characterData.intelligence, labelStyle);
        EditorGUILayout.LabelField("Vitality: " + _characterData.vitality, labelStyle);
        EditorGUILayout.LabelField("Focus: " + _characterData.focus, labelStyle);
        EditorGUILayout.LabelField("Charisma: " + _characterData.charisma, labelStyle);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Stat Points: " + _characterData.statPoints, labelStyle);
        EditorGUILayout.LabelField("Level: " + _characterData.level);
        DrawProgressBar("Experience", _characterData.experience, _levelData.GetRequiredExperience(_characterData.level), Color.yellow);
    }

    private void DrawProgressBar(string label, int value, int maxValue, Color backgroundColor)
    {
        float progress = (float)value / maxValue;
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");

        // Set the background color
        EditorGUI.DrawRect(rect, backgroundColor);

        EditorGUI.ProgressBar(rect, progress, label + ": " + value + " / " + maxValue);
    }

    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; ++i)
        {
            pixels[i] = color;
        }
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
#endif
}