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
        characterDataSO = CharacterDataSO.CreateInstance<CharacterDataSO>();
        SetStats();
    }

    private void OnCharacterEquippedOrUnequippedItem(object sender, EventArgs e)
    {
        characterDataSO.CalculateDerivedStats();
        Character.Instance.GetCharacterData().CalculateDerivedStats();

        // Update Equipped Weapon Id
        CharacterInventory.Instance.ItemEquipped();
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