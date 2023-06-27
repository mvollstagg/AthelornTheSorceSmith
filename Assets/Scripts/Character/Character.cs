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
                    _characterData.currentHealth = Mathf.Clamp(_characterData.currentHealth, 0, _characterData.maxHealth);
                }
                else if (trait.Type == TraitType.Mana)
                {
                    _characterData.currentMana += trait.Value * traitStatusMultiplier;
                    _characterData.currentMana = Mathf.Clamp(_characterData.currentMana, 0, _characterData.maxMana);
                }
                else if (trait.Type == TraitType.Stamina)
                {
                    _characterData.currentStamina += trait.Value * traitStatusMultiplier;
                    _characterData.currentStamina = Mathf.Clamp(_characterData.currentStamina, 0, _characterData.maxStamina);
                }
            }
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

    public Dictionary<string, CharacterStat> GetStats()
    {
        var stats = new Dictionary<string, CharacterStat>();
        stats["Health"] = new CharacterStat("Health", _characterData.currentHealth, _characterData.maxHealth);
        stats["Mana"] = new CharacterStat("Mana", _characterData.currentMana, _characterData.maxMana);
        stats["Stamina"] = new CharacterStat("Stamina", _characterData.currentStamina, _characterData.maxStamina);
        stats["Strength"] = new CharacterStat("Strength", _characterData.strength, _characterData.strength);
        stats["Dexterity"] = new CharacterStat("Dexterity", _characterData.dexterity, _characterData.dexterity);
        stats["Intelligence"] = new CharacterStat("Intelligence", _characterData.intelligence, _characterData.intelligence);
        stats["Speed"] = new CharacterStat("Speed", _characterData.speed, _characterData.speed);
        stats["Vitality"] = new CharacterStat("Vitality", _characterData.vitality, _characterData.vitality);
        stats["Endurance"] = new CharacterStat("Endurance", _characterData.endurance, _characterData.endurance);
        stats["Max Health"] = new CharacterStat("Max Health", _characterData.maxHealth, _characterData.maxHealth);
        stats["Max Stamina"] = new CharacterStat("Max Stamina", _characterData.maxStamina, _characterData.maxStamina);
        stats["Max Mana"] = new CharacterStat("Max Mana", _characterData.maxMana, _characterData.maxMana);
        stats["Attack"] = new CharacterStat("Attack", _characterData.attack, _characterData.attack);
        stats["Defence"] = new CharacterStat("Defence", _characterData.defence, _characterData.defence);
        stats["Stat Points"] = new CharacterStat("Stat Points", _characterData.statPoints, byte.MaxValue);

        return stats;
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
                case "Speed":
                    _characterData.speed += points;
                    break;
                case "Vitality":
                    _characterData.vitality += points;
                    break;
                case "Endurance":
                    _characterData.endurance += points;
                    break;
            }
        }
    }

    public void GainExperience(int experience)
    {
        _characterData.GainExperience(experience);

        if (_characterData.experience >= RequiredExperienceForNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _characterData.LevelUp();
        RequiredExperienceForNextLevel = _levelData.GetRequiredExperience(_characterData.level);

        // Perform any additional level up actions here
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
        DrawProgressBar("Health", _characterData.currentHealth, _characterData.maxHealth, Color.red);
        DrawProgressBar("Mana", _characterData.currentMana, _characterData.maxMana, Color.blue);
        DrawProgressBar("Stamina", _characterData.currentStamina, _characterData.maxStamina, new Color(1f, 0.7f, 0f));
        EditorGUILayout.LabelField("Attack: " + _characterData.attack, labelStyle);
        EditorGUILayout.LabelField("Defence: " + _characterData.defence, labelStyle);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Strength: " + _characterData.strength, labelStyle);
        EditorGUILayout.LabelField("Dexterity: " + _characterData.dexterity, labelStyle);
        EditorGUILayout.LabelField("Intelligence: " + _characterData.intelligence, labelStyle);
        EditorGUILayout.LabelField("Speed: " + _characterData.speed, labelStyle);
        EditorGUILayout.LabelField("Vitality: " + _characterData.vitality, labelStyle);
        EditorGUILayout.LabelField("Endurance: " + _characterData.endurance, labelStyle);

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