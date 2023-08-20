using System.Collections.Generic;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using Scripts.Core;
using System.Linq;
using System;

public class CharacterManager : Singleton<CharacterManager>
{
    public CharacterDataSO characterDataSO;
    private Dictionary<string, CharacterStat> characterStats;
    
    void Awake()
    {
        EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM, OnCharacterEquippedOrUnequippedItem);
    }

    void Start()
    {
        SetStats();
    }

    private void OnCharacterEquippedOrUnequippedItem(object sender, EventArgs e)
    {
        characterDataSO = Character.Instance.GetCharacterData();
        var equippedItems = InventoryManager.Instance._equipments;

        // Collect positive and negative modifiers from equipped items
        List<ItemTrait> positiveStatTraits = new List<ItemTrait>();
        List<ItemTrait> negativeStatTraits = new List<ItemTrait>();

        foreach (var equippedItem in equippedItems.Values)
        {
            positiveStatTraits.AddRange(equippedItem.Item.traits.Where(y => y.Status == TraitStatus.Positive));
            negativeStatTraits.AddRange(equippedItem.Item.traits.Where(y => y.Status == TraitStatus.Negative));
        }

        // Reset base stats
        characterDataSO.strength = 3;
        characterDataSO.vitality = 3;
        characterDataSO.intelligence = 3;
        characterDataSO.focus = 3;
        characterDataSO.dexterity = 3;
        characterDataSO.charisma = 3;

        // Calculate base stats
        characterDataSO.strength += (int)CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Strength);
        characterDataSO.vitality += (int)CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Vitality);
        characterDataSO.intelligence += (int)CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Intelligence);
        characterDataSO.focus += (int)CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Focus);
        characterDataSO.dexterity += (int)CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Dexterity);
        characterDataSO.charisma += (int)CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Charisma);

        characterDataSO.CalculateDerivedStats();

        // Calculate derived stats
        characterDataSO.physicalDamage += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.PhysicalDamage);
        characterDataSO.physicalDefence += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.PhysicalDefence);
        characterDataSO.magicalDamage += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.MagicalDamage);
        characterDataSO.magicalDefence += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.MagicalDefence);

        characterDataSO.maxHealth += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Health);
        characterDataSO.maxMana += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Mana);
        characterDataSO.maxStamina += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Stamina);

        characterDataSO.criticalStrikeChance += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.CriticalStrikeChance);
        characterDataSO.criticalStrikeDamage += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.CriticalStrikeDamage);
        characterDataSO.accuracy += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.Accuracy);
        characterDataSO.dodgeChance += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.DodgeChance);
        characterDataSO.blockChance += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.BlockChance);
        characterDataSO.influenceBonus += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.InfluenceBonus);
        characterDataSO.negotiationBonus += CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.NegotiationBonus);

        characterDataSO.fireResistance = CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.FireResistance);
        characterDataSO.waterResistance = CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.WaterResistance);
        characterDataSO.earthResistance = CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.EarthResistance);
        characterDataSO.airResistance = CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.AirResistance);
        characterDataSO.poisonResistance = CalculateTraitValue(positiveStatTraits, negativeStatTraits, TraitType.PoisonResistance);
    }


    private float CalculateTraitValue(List<ItemTrait> positiveTraits, List<ItemTrait> negativeTraits, TraitType traitType)
    {
        float positiveValue = positiveTraits.Where(t => t.Type == traitType).Sum(t => t.Value);
        float negativeValue = negativeTraits.Where(t => t.Type == traitType).Sum(t => t.Value);

        return positiveValue - negativeValue;
    }

    public void SetStats()
    {
        // Get the current stats of the character
        var currentStats = Character.Instance.GetCharacterData();

        // Create a new CharacterDataSO instance
        characterDataSO = CharacterDataSO.CreateInstance<CharacterDataSO>();

        // Set the values of the CharacterDataSO instance
        characterDataSO.strength = currentStats.strength;
        characterDataSO.dexterity = currentStats.dexterity;
        characterDataSO.intelligence = currentStats.intelligence;
        characterDataSO.vitality = currentStats.vitality;
        characterDataSO.focus = currentStats.focus;
        characterDataSO.charisma = currentStats.charisma;
        characterDataSO.currentHealth = currentStats.currentHealth;
        characterDataSO.currentMana = currentStats.currentMana;
        characterDataSO.currentStamina = currentStats.currentStamina;

        characterDataSO.fireResistance = currentStats.fireResistance;
        characterDataSO.waterResistance = currentStats.waterResistance;
        characterDataSO.earthResistance = currentStats.earthResistance;
        characterDataSO.airResistance = currentStats.airResistance;
        characterDataSO.poisonResistance = currentStats.poisonResistance;

        characterDataSO.statPoints = currentStats.statPoints;

        // Calculate the derived stats
        characterDataSO.CalculateDerivedStats();

        CharacterUIManager.Instance.UpdateInGameCharacterStatUI(characterDataSO.GetStats());
    }

    public void RevertChanges()
    {
        SetStats();
        CharacterUIManager.Instance.UpdateIncreaseDecreaseButtonsVisibility();
    }

    public void ApplyChanges()
    {
        var currentStats = Character.Instance.GetCharacterData();

        currentStats.strength = characterDataSO.strength;
        currentStats.dexterity = characterDataSO.dexterity;
        currentStats.intelligence = characterDataSO.intelligence;
        currentStats.vitality = characterDataSO.vitality;
        currentStats.focus = characterDataSO.focus;
        currentStats.charisma = characterDataSO.charisma;

        currentStats.statPoints = characterDataSO.statPoints;

        currentStats.CalculateDerivedStats();

        SetStats();
        CharacterUIManager.Instance.UpdateIncreaseDecreaseButtonsVisibility();

        currentStats.SaveStats();
    }
}