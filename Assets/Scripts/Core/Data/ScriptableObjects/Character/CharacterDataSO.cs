using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/Character/CharacterDataSO", order = 1)]
public class CharacterDataSO : ScriptableObject
{
    #region Base Stats
    [SerializeField] public int strength;      // Strength is a measure of your raw physical power and the force behind your attacks.
    [SerializeField] public int dexterity;     // Dexterity reflects your agility, precision, and quick reflexes.
    [SerializeField] public int intelligence;  // Intelligence represents your knowledge, understanding, and affinity for magic.
    [SerializeField] public int vitality;      // Vitality represents your endurance, resilience, and overall toughness.
    [SerializeField] public int focus;         // Focus is a measure of your mental and spiritual energy. 
    [SerializeField] public int charisma;      // Charisma reflects your charm, presence, and persuasive abilities.
    #endregion

    #region Base Values
    [SerializeField] public int physicalDamageBaseValue = 10;
    [SerializeField] public int physicalDefenceBaseValue = 25;
    [SerializeField] public int magicalDamageBaseValue = 10;
    [SerializeField] public int magicalDefenceBaseValue = 25;
    [SerializeField] public int hpBaseValue = 50;
    [SerializeField] public int mpBaseValue = 30;
    [SerializeField] public int spBaseValue = 100;
    [SerializeField] public int criticalStrikeChanceBaseValue = 5;
    [SerializeField] public int criticalStrikeDamageBaseValue = 50;
    [SerializeField] public int accuracyBaseValue = 40;
    [SerializeField] public int dodgeChanceBaseValue = 5;
    [SerializeField] public int blockChanceBaseValue = 0;
    [SerializeField] public int influenceBonusBaseValue = 1;
    [SerializeField] public int negotiationBonusBaseValue = 1;
    #endregion

    #region Derived Stats
    public double physicalDamage { get; set; }                      // PhysicalDamageBaseValue + ( PhysicalDamageBaseValue * Strength * 0.4 )
    public double physicalDefence { get; set; }                     // PhysicalDefenceBaseValue + ( PhysicalDefenceBaseValue * Vitality * 0.3 )
    public double magicalDamage { get; set; }                       // MagicalDamageBaseValue + ( MagicalDamageBaseValue * Intelligence * 0.5 )
    public double magicalDefence { get; set; }                      // MagicalDefenceBaseValue + ( MagicalDefenceBaseValue * Intelligence * 0.3 )

    public double maxHealth { get; set; }                           // HpBaseValue + ( HpBaseValue * Vitality * 0.1 );
    public double currentHealth { get; set; }                       // Current health points
    public double maxMana { get; set; }                             // MpBaseValue + ( MpBaseValue * Intelligence * 0.1 );
    public double currentMana { get; set; }                         // Current mana points
    public double maxStamina { get; set; }                          // SpBaseValue + ( SpBaseValue * Focus * 0.01 );
    public double currentStamina { get; set; }                      // Current stamina points

    public double criticalStrikeChance { get; set; }                // CriticalStrikeChanceBaseValue + ( Dexterity * 4 );
    public double criticalStrikeDamage { get; set; }                // CriticalStrikeDamageBaseValue + ( Dexterity * 3.5 );
    public double accuracy { get; set; }                            // AccuracyBaseValue + ( Dexterity * 2 );
    public double dodgeChance { get; set; }                         // DodgeChanceBaseValue + ( Dexterity * 2 );
    public double blockChance { get; set; }                         // BlockChanceBaseValue + ( Dexterity * 2 );
    public double influenceBonus { get; set; }                      // InfluenceBonusBaseValue + ( Charisma * 0.5 );
    public double negotiationBonus { get; set; }                    // NegotiationBonusBaseValue + ( Charisma * 0.5 );

    public double fireResistance { get; set; }                      // FireResistanceBaseValue + ( Vitality * 0.5 );
    public double waterResistance { get; set; }                     // WaterResistanceBaseValue + ( Vitality * 0.5 );
    public double earthResistance { get; set; }                     // EarthResistanceBaseValue + ( Vitality * 0.5 );
    public double airResistance { get; set; }                       // AirResistanceBaseValue + ( Vitality * 0.5 );
    public double poisonResistance { get; set; }                    // PoisonResistanceBaseValue + ( Vitality * 0.5 );
    #endregion 

    #region Character Values
    public int level { get; private set; }                          // Current level
    public int experience { get; private set; }                     // Current experience
    public int statPoints { get; set; }                             // Stat points available to spend
    #endregion

    private const string INITIALIZED_KEY = "IsInitialized"; // Key for PlayerPrefs
    
    void Awake()
    {
        CalculateDerivedStats();
        if (!PlayerPrefs.HasKey(INITIALIZED_KEY))
        {
            InitializeStats();
            PlayerPrefs.SetInt(INITIALIZED_KEY, 1);
        }

        LoadStats();
        CalculateDerivedStats();
    }
    
    void OnApplicationQuit()
    {
        SaveStats();
    }

    private void InitializeStats()
    {
        ResetCharacterData();
        SaveStats();
    }

    private void LoadStats()
    {
        // Load saved stats values from PlayerPrefs (if any)
        currentHealth = PlayerPrefs.GetFloat("CurrentHealth");
        currentMana = PlayerPrefs.GetFloat("CurrentMana");
        currentStamina = PlayerPrefs.GetFloat("CurrentStamina");

        strength = PlayerPrefs.GetInt("Strength");
        dexterity = PlayerPrefs.GetInt("Dexterity");
        intelligence = PlayerPrefs.GetInt("Intelligence");
        vitality = PlayerPrefs.GetInt("Vitality");
        focus = PlayerPrefs.GetInt("Focus");
        charisma = PlayerPrefs.GetInt("Charisma");

        level = PlayerPrefs.GetInt("Level");
        experience = PlayerPrefs.GetInt("Experience");
        statPoints = PlayerPrefs.GetInt("StatPoints");
    }

    // Save the stats whenever necessary, for example, when the game is saved
    public void SaveStats()
    {
        PlayerPrefs.SetFloat("CurrentHealth", (float)currentHealth);
        PlayerPrefs.SetFloat("CurrentMana", (float)currentMana);
        PlayerPrefs.SetFloat("CurrentStamina", (float)currentStamina);

        PlayerPrefs.SetInt("Strength", strength);
        PlayerPrefs.SetInt("Dexterity", dexterity);
        PlayerPrefs.SetInt("Intelligence", intelligence);
        PlayerPrefs.SetInt("Vitality", vitality);
        PlayerPrefs.SetInt("Focus", focus);
        PlayerPrefs.SetInt("Charisma", charisma);
        
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Experience", experience);
        PlayerPrefs.SetInt("StatPoints", statPoints);
        PlayerPrefs.Save();
    }

    public void LevelUp()
    {
        experience -= Character.Instance.GetRequiredExperienceForLevel(level);
        
        level++;
        statPoints++;

    }

    public void GainExperience(int experience)
    {
        var requiredExperienceForNextLevel = Character.Instance.GetRequiredExperienceForLevel(level);

        if (requiredExperienceForNextLevel == 0)
        {
            Debug.Log("Maximum level reached.");
            return;
        }

        this.experience += experience;

        if (this.experience >= requiredExperienceForNextLevel)
        {
            LevelUp();
        }

        SaveStats();
    }

    public void SpendStatPoint(CharacterStatType stat)
    {
        if (statPoints <= 0)
        {
            Debug.Log("No stat points available to spent.");
            return;
        }

        switch (stat.ToString())
        {
            case "Strength":
                strength++;
                break;
            case "Dexterity":
                dexterity++;
                break;
            case "Intelligence":
                intelligence++;
                break;
            case "Vitality":
                vitality++;
                break;
            case "Focus":
                focus++;
                break;
            case "Charisma":
                charisma++;
                break;
        }

        statPoints--;
        CalculateDerivedStats();
    }

    public void ResetCharacterData()
    {
        Debug.Log("Resetting character data");
        strength = 3;
        dexterity = 3;
        intelligence = 3;
        vitality = 3;
        focus = 3;
        charisma = 3;

        CalculateDerivedStats();

        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
        level = 1;
        experience = 0;
        statPoints = 0;
    }

    // Calculate and update the derived stats
    public void CalculateDerivedStats()
    {
        var equippedItems = InventoryManager.Instance._equipments;
        // Collect positive and negative modifiers from equipped items
        List<ItemTrait> positiveStatTraits = new List<ItemTrait>();
        List<ItemTrait> negativeStatTraits = new List<ItemTrait>();
        foreach (var equippedItem in equippedItems.Values)
        {
            positiveStatTraits.AddRange(equippedItem.Item.traits.Where(y => y.Status == TraitStatus.Positive));
            negativeStatTraits.AddRange(equippedItem.Item.traits.Where(y => y.Status == TraitStatus.Negative));
        }

        // Calculate base stats (without equipped modifiers)
        int baseStrength = 3;
        int baseVitality = 3;
        int baseIntelligence = 3;
        int baseFocus = 3;
        int baseDexterity = 3;
        int baseCharisma = 3;

        // Apply equipped modifiers
        int equippedStrengthModifier = positiveStatTraits.Where(t => t.Type == TraitType.Strength).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Strength).Sum(t => t.Value);
        int equippedVitalityModifier = positiveStatTraits.Where(t => t.Type == TraitType.Vitality).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Vitality).Sum(t => t.Value);
        int equippedIntelligenceModifier = positiveStatTraits.Where(t => t.Type == TraitType.Intelligence).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Intelligence).Sum(t => t.Value);
        int equippedFocusModifier = positiveStatTraits.Where(t => t.Type == TraitType.Focus).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Focus).Sum(t => t.Value);
        int equippedDexterityModifier = positiveStatTraits.Where(t => t.Type == TraitType.Dexterity).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Dexterity).Sum(t => t.Value);
        int equippedCharismaModifier = positiveStatTraits.Where(t => t.Type == TraitType.Charisma).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Charisma).Sum(t => t.Value);

        double equippedPhysicalDamageModifier = positiveStatTraits.Where(t => t.Type == TraitType.PhysicalDamage).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.PhysicalDamage).Sum(t => t.Value);
        double equippedPhysicalDefenceModifier = positiveStatTraits.Where(t => t.Type == TraitType.PhysicalDefence).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.PhysicalDefence).Sum(t => t.Value);
        double equippedMagicalDamageModifier = positiveStatTraits.Where(t => t.Type == TraitType.MagicalDamage).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.MagicalDamage).Sum(t => t.Value);
        double equippedMagicalDefenceModifier = positiveStatTraits.Where(t => t.Type == TraitType.MagicalDefence).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.MagicalDefence).Sum(t => t.Value);

        double equippedHealthModifier = positiveStatTraits.Where(t => t.Type == TraitType.Health).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Health).Sum(t => t.Value);
        double equippedManaModifier = positiveStatTraits.Where(t => t.Type == TraitType.Mana).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Mana).Sum(t => t.Value);
        double equippedStaminaModifier = positiveStatTraits.Where(t => t.Type == TraitType.Stamina).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Stamina).Sum(t => t.Value);

        double equippedCriticalStrikeChanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.CriticalStrikeChance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.CriticalStrikeChance).Sum(t => t.Value);
        double equippedCriticalStrikeDamageModifier = positiveStatTraits.Where(t => t.Type == TraitType.CriticalStrikeDamage).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.CriticalStrikeDamage).Sum(t => t.Value);
        double equippedAccuracyModifier = positiveStatTraits.Where(t => t.Type == TraitType.Accuracy).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.Accuracy).Sum(t => t.Value);
        double equippedDodgeChanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.DodgeChance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.DodgeChance).Sum(t => t.Value);
        double equippedBlockChanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.BlockChance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.BlockChance).Sum(t => t.Value);
        double equippedInfluenceBonusModifier = positiveStatTraits.Where(t => t.Type == TraitType.InfluenceBonus).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.InfluenceBonus).Sum(t => t.Value);
        double equippedNegotiationBonusModifier = positiveStatTraits.Where(t => t.Type == TraitType.NegotiationBonus).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.NegotiationBonus).Sum(t => t.Value);

        double equippedFireResistanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.FireResistance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.FireResistance).Sum(t => t.Value);
        double equippedWaterResistanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.WaterResistance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.WaterResistance).Sum(t => t.Value);
        double equippedEarthResistanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.EarthResistance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.EarthResistance).Sum(t => t.Value);
        double equippedAirResistanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.AirResistance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.AirResistance).Sum(t => t.Value);
        double equippedPoisonResistanceModifier = positiveStatTraits.Where(t => t.Type == TraitType.PoisonResistance).Sum(t => t.Value) - negativeStatTraits.Where(t => t.Type == TraitType.PoisonResistance).Sum(t => t.Value);

        physicalDamage = physicalDamageBaseValue + ( physicalDamageBaseValue * (strength + equippedStrengthModifier) * 0.4 ) + equippedPhysicalDamageModifier;
        physicalDefence = physicalDefenceBaseValue + ( physicalDefenceBaseValue * (vitality + equippedVitalityModifier) * 0.3 ) + equippedPhysicalDefenceModifier;
        magicalDamage = magicalDamageBaseValue + ( magicalDamageBaseValue * (intelligence + equippedIntelligenceModifier) * 0.5 ) + equippedMagicalDamageModifier;
        magicalDefence = magicalDefenceBaseValue + ( magicalDefenceBaseValue * (intelligence + equippedIntelligenceModifier) * 0.3 ) + equippedMagicalDefenceModifier;
        
        maxHealth = hpBaseValue + ( hpBaseValue * (vitality + equippedVitalityModifier) * 0.1 ) + equippedHealthModifier;
        maxMana = mpBaseValue + ( mpBaseValue * (intelligence + equippedIntelligenceModifier) * 0.1 ) + equippedManaModifier;
        maxStamina = spBaseValue + ( spBaseValue * (focus + equippedFocusModifier) * 0.01 ) + equippedStaminaModifier;

        criticalStrikeChance = criticalStrikeChanceBaseValue + ( dexterity + equippedDexterityModifier ) * 4 + equippedCriticalStrikeChanceModifier;
        criticalStrikeDamage = criticalStrikeDamageBaseValue + ( dexterity + equippedDexterityModifier ) * 3.5 + equippedCriticalStrikeDamageModifier;
        accuracy = accuracyBaseValue + ( dexterity + equippedDexterityModifier ) * 2 + equippedAccuracyModifier;
        dodgeChance = dodgeChanceBaseValue + ( dexterity + equippedDexterityModifier ) * 2 + equippedDodgeChanceModifier;
        blockChance = blockChanceBaseValue + ( dexterity + equippedDexterityModifier ) * 2 + equippedBlockChanceModifier;
        influenceBonus = influenceBonusBaseValue + ( charisma + equippedCharismaModifier ) * 0.5 + equippedInfluenceBonusModifier;
        negotiationBonus = negotiationBonusBaseValue + ( charisma + equippedCharismaModifier ) * 0.5 + equippedNegotiationBonusModifier;

        fireResistance = equippedFireResistanceModifier;
        waterResistance = equippedWaterResistanceModifier;
        earthResistance = equippedEarthResistanceModifier;
        airResistance = equippedAirResistanceModifier;
        poisonResistance = equippedPoisonResistanceModifier;

        // Make sure current health, mana and stamina are not greater than their maximum values
        currentHealth = Math.Min(currentHealth, maxHealth);
        currentMana = Math.Min(currentMana, maxMana);
        currentStamina = Math.Min(currentStamina, maxStamina);
    }

    public Dictionary<string, CharacterStat> GetStats()
    {
        var stats = new Dictionary<string, CharacterStat>();

        stats["Stat Points"] = new CharacterStat("Stat Points", statPoints, byte.MaxValue);

        stats["Strength"] = new CharacterStat("Strength", strength, strength);
        stats["Dexterity"] = new CharacterStat("Dexterity", dexterity, dexterity);
        stats["Intelligence"] = new CharacterStat("Intelligence", intelligence, intelligence);
        stats["Vitality"] = new CharacterStat("Vitality", vitality, vitality);
        stats["Focus"] = new CharacterStat("Focus", focus, focus);
        stats["Charisma"] = new CharacterStat("Charisma", charisma, charisma);
        
        stats["Health"] = new CharacterStat("Health", (int)currentHealth, (int)maxHealth);
        stats["Mana"] = new CharacterStat("Mana", (int)currentMana, (int)maxMana);
        stats["Stamina"] = new CharacterStat("Stamina", (int)currentStamina, (int)maxStamina);
        
        stats["Level"] = new CharacterStat("Level", (int)level, (int)level);
        stats["Experience"] = new CharacterStat("Experience", (int)experience, (int)Character.Instance.GetRequiredExperienceForLevel(level));

        return stats;
    }

    public Dictionary<string, CharacterStat> GetBaseStats()
    {
        var stats = new Dictionary<string, CharacterStat>();

        stats["Strength"] = new CharacterStat("Strength", strength, strength);
        stats["Dexterity"] = new CharacterStat("Dexterity", dexterity, dexterity);
        stats["Intelligence"] = new CharacterStat("Intelligence", intelligence, intelligence);
        stats["Vitality"] = new CharacterStat("Vitality", vitality, vitality);
        stats["Focus"] = new CharacterStat("Focus", focus, focus);
        stats["Charisma"] = new CharacterStat("Charisma", charisma, charisma);

        return stats;
    }

    internal void IncreaseStat(string statName)
    {
        switch (statName)
        {
            case "Strength":
                strength++;
                break;
            case "Dexterity":
                dexterity++;
                break;
            case "Intelligence":
                intelligence++;
                break;
            case "Vitality":
                vitality++;
                break;
            case "Focus":
                focus++;
                break;
            case "Charisma":
                charisma++;
                break;
            default:
                Debug.LogWarning("Invalid statName provided: " + statName);
                break;
        }

        // Recalculate derived stats after increasing the stat
        CalculateDerivedStats();
    }

    internal void DecreaseStat(string statName)
    {
        switch (statName)
        {
            case "Strength":
                strength--;
                break;
            case "Dexterity":
                dexterity--;
                break;
            case "Intelligence":
                intelligence--;
                break;
            case "Vitality":
                vitality--;
                break;
            case "Focus":
                focus--;
                break;
            case "Charisma":
                charisma--;
                break;
            default:
                Debug.LogWarning("Invalid statName provided: " + statName);
                break;
        }

        // Recalculate derived stats after decreasing the stat
        CalculateDerivedStats();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterDataSO))]
    public class CharacterDataSOEditor : Editor
    {
        private SerializedProperty strengthProperty;
        private SerializedProperty dexterityProperty;
        private SerializedProperty intelligenceProperty;
        private SerializedProperty vitalityProperty;
        private SerializedProperty focusProperty;
        private SerializedProperty charismaProperty;

        private GUIContent decreaseButtonContent;
        private GUIContent increaseButtonContent;
        

        private const float buttonWidth = 20f;
        private const float inputFieldWidth = 50f;

        private void OnEnable()
        {
            strengthProperty = serializedObject.FindProperty("strength");
            dexterityProperty = serializedObject.FindProperty("dexterity");
            intelligenceProperty = serializedObject.FindProperty("intelligence");
            vitalityProperty = serializedObject.FindProperty("vitality");
            focusProperty = serializedObject.FindProperty("focus");
            charismaProperty = serializedObject.FindProperty("charisma");

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
            DrawBaseStat("Vitality", vitalityProperty);
            DrawBaseStat("Focus", focusProperty);
            DrawBaseStat("Charisma", charismaProperty);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Other Stats", EditorStyles.boldLabel);
            // EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.DoubleField("Level", ((CharacterDataSO)target).level);
            EditorGUILayout.DoubleField("Experience", ((CharacterDataSO)target).experience);
            EditorGUILayout.DoubleField("Stat Points", ((CharacterDataSO)target).statPoints);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Derived Stats", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.DoubleField("Physcial Damage", ((CharacterDataSO)target).physicalDamage);
            EditorGUILayout.DoubleField("Physical Defence", ((CharacterDataSO)target).physicalDefence);
            EditorGUILayout.DoubleField("Magical Damage", ((CharacterDataSO)target).magicalDamage);
            EditorGUILayout.DoubleField("Magical Defence", ((CharacterDataSO)target).magicalDefence);

            EditorGUILayout.Space();

            EditorGUILayout.DoubleField("Max Health", ((CharacterDataSO)target).maxHealth);
            EditorGUILayout.DoubleField("Max Stamina", ((CharacterDataSO)target).maxStamina);
            EditorGUILayout.DoubleField("Max Mana", ((CharacterDataSO)target).maxMana);

            EditorGUILayout.Space();

            EditorGUILayout.DoubleField("Critical Strike Chance", ((CharacterDataSO)target).criticalStrikeChance);
            EditorGUILayout.DoubleField("Critical Strike Damage", ((CharacterDataSO)target).criticalStrikeDamage);
            EditorGUILayout.DoubleField("Accuracy", ((CharacterDataSO)target).accuracy);
            EditorGUILayout.DoubleField("Dodge Chance", ((CharacterDataSO)target).dodgeChance);
            EditorGUILayout.DoubleField("Block Chance", ((CharacterDataSO)target).blockChance);
            EditorGUILayout.DoubleField("Influence Bonus", ((CharacterDataSO)target).influenceBonus);
            EditorGUILayout.DoubleField("Negotiation Bonus", ((CharacterDataSO)target).negotiationBonus);

            EditorGUILayout.Space();

            EditorGUILayout.DoubleField("Fire Resistance", ((CharacterDataSO)target).fireResistance);
            EditorGUILayout.DoubleField("Water Resistance", ((CharacterDataSO)target).waterResistance);
            EditorGUILayout.DoubleField("Earth Resistance", ((CharacterDataSO)target).earthResistance);
            EditorGUILayout.DoubleField("Air Resistance", ((CharacterDataSO)target).airResistance);
            EditorGUILayout.DoubleField("Poison Resistance", ((CharacterDataSO)target).poisonResistance);

            EditorGUILayout.Space();

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

            int currentValue = property.intValue;
            int availableStatPoints = ((CharacterDataSO)target).statPoints;

            // Decrease button
            EditorGUI.BeginDisabledGroup(currentValue <= 0);
            if (GUILayout.Button(decreaseButtonContent, EditorStyles.miniButtonLeft, GUILayout.Width(buttonWidth)))
            {
                property.intValue = Mathf.Max(0, currentValue - 1);
                ((CharacterDataSO)target).statPoints++;
            }
            EditorGUI.EndDisabledGroup();

            // Increase button
            EditorGUI.BeginDisabledGroup(availableStatPoints <= 0);
            if (GUILayout.Button(increaseButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(buttonWidth)))
            {
                property.intValue++;
                ((CharacterDataSO)target).statPoints--;
            }
            EditorGUI.EndDisabledGroup();

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