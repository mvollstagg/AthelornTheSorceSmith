using System.Collections.Generic;
using Scripts.Entities.Class;
using Scripts.Core;

public class CharacterManager : Singleton<CharacterManager>
{
    public CharacterDataSO characterDataSO;
    private Dictionary<string, CharacterStat> characterStats;

    void Start()
    {
        SetStats();
    }

    // void OnDisable()
    // {
    //     RevertChanges();
    // }

    // void OnApplicationQuit()
    // {
    //     RevertChanges();
    // }

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