using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/Character/CharacterDataSO", order = 1)]
public class CharacterDataSO : ScriptableObject
{
    // Base stats
    [SerializeField] public int strength;      // Physical power
    [SerializeField] public int dexterity;     // Agility and reflexes
    [SerializeField] public int intelligence;  // Mental acuity
    [SerializeField] public int speed;         // Movement speed
    [SerializeField] public int vitality;      // Endurance and resilience
    [SerializeField] public int endurance;     // Defensive capability

    // Derived stats
    public int maxHealth { get; private set; }  // Maximum health points
    public int currentHealth { get; set; }  // Current health points
    public int maxStamina { get; private set; } // Maximum stamina points
    public int currentStamina { get; set; } // Current stamina points
    public int maxMana { get; private set; }    // Maximum mana points
    public int currentMana { get; set; }    // Current mana points
    public int attack { get; private set; }     // Attack power
    public int defence { get; private set; }    // Defence power

    public int level { get; private set; }    // Current level
    public int experience { get; private set; }    // Current experience
    public int statPoints { get; set; }    // Stat points available to spend

    private bool _isInitialized = false;

    public void LevelUp()
    {
        experience -= Character.Instance.RequiredExperienceForNextLevel;
        
        level++;
        statPoints++;

    }

    public void GainExperience(int experience)
    {
        this.experience += experience;
    }

    void OnEnable()
    {
        CalculateDerivedStats();

        if (!_isInitialized)
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
            level = 1;
            experience = 0;
            statPoints = 0;
            _isInitialized = true;
        }
    }

    public void ResetCharacterData()
    {
        Debug.Log("Resetting character data");
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
        level = 1;
        experience = 0;
        statPoints = 0;
        CalculateDerivedStats();
    }

    // Calculate and update the derived stats
    public void CalculateDerivedStats()
    {
        // Calculate max health based on vitality
        maxHealth = vitality * 10;

        // Calculate max stamina based on speed
        maxStamina = speed * 5;

        // Calculate max mana based on intelligence
        maxMana = intelligence * 8;

        // Calculate attack based on strength and dexterity
        attack = (strength + dexterity) / 2;

        // Calculate defence based on vitality and endurance
        defence = (vitality + endurance) / 2;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterDataSO))]
    public class CharacterDataSOEditor : Editor
    {
        private SerializedProperty strengthProperty;
        private SerializedProperty dexterityProperty;
        private SerializedProperty intelligenceProperty;
        private SerializedProperty speedProperty;
        private SerializedProperty vitalityProperty;
        private SerializedProperty enduranceProperty;

        private GUIContent decreaseButtonContent;
        private GUIContent increaseButtonContent;

        private const float buttonWidth = 20f;
        private const float inputFieldWidth = 50f;

        private void OnEnable()
        {
            strengthProperty = serializedObject.FindProperty("strength");
            dexterityProperty = serializedObject.FindProperty("dexterity");
            intelligenceProperty = serializedObject.FindProperty("intelligence");
            speedProperty = serializedObject.FindProperty("speed");
            vitalityProperty = serializedObject.FindProperty("vitality");
            enduranceProperty = serializedObject.FindProperty("endurance");

            decreaseButtonContent = EditorGUIUtility.IconContent("Toolbar Minus");
            increaseButtonContent = EditorGUIUtility.IconContent("Toolbar Plus");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);
            DrawBaseStat("Strength", strengthProperty);
            DrawBaseStat("Dexterity", dexterityProperty);
            DrawBaseStat("Intelligence", intelligenceProperty);
            DrawBaseStat("Speed", speedProperty);
            DrawBaseStat("Vitality", vitalityProperty);
            DrawBaseStat("Endurance", enduranceProperty);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Derived Stats", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Max Health", ((CharacterDataSO)target).maxHealth);
            EditorGUILayout.IntField("Max Stamina", ((CharacterDataSO)target).maxStamina);
            EditorGUILayout.IntField("Max Mana", ((CharacterDataSO)target).maxMana);
            EditorGUILayout.IntField("Attack", ((CharacterDataSO)target).attack);
            EditorGUILayout.IntField("Defence", ((CharacterDataSO)target).defence);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Reset Character Data"))
            {
                ((CharacterDataSO)target).ResetCharacterData();
            }
        }

        private void DrawBaseStat(string label, SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(100));
            GUI.enabled = false;
            EditorGUIUtility.fieldWidth = inputFieldWidth;
            EditorGUILayout.PropertyField(property, GUIContent.none);
            GUI.enabled = true;
            DrawIncreaseDecreaseButtons(property);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawIncreaseDecreaseButtons(SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();

            // Decrease button
            if (GUILayout.Button(decreaseButtonContent, EditorStyles.miniButtonLeft, GUILayout.Width(buttonWidth)))
            {
                property.intValue = Mathf.Max(0, property.intValue - 1);
            }

            // Increase button
            if (GUILayout.Button(increaseButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(buttonWidth)))
            {
                property.intValue++;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ((CharacterDataSO)target).CalculateDerivedStats();
                EditorUtility.SetDirty(target);
            }
        }
    }
#endif
}