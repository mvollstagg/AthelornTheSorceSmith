using Scripts.Core;
using UnityEngine;

public class CharacterCombatModeSwitch : Singleton<CharacterCombatModeSwitch>
{
    public string combatZoneTag = "CombatArea"; // Tag assigned to the combat zone cube

    public bool isInCombatMode = false; // Track whether character is in combat mode

    void OnTriggerEnter(Collider other)
    {
        // Check if the character enters the combat zone
        if (other.CompareTag(combatZoneTag))
        {
            // Switch to combat mode
            SwitchToCombatMode();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the character exits the combat zone
        if (other.CompareTag(combatZoneTag))
        {
            // Switch to normal mode
            SwitchToNormalMode();
        }
    }

    void SwitchToCombatMode()
    {
        // Example: Set character to combat mode
        Debug.Log("Character is in combat mode");
        // Add your combat mode logic here
        isInCombatMode = true;
    }

    void SwitchToNormalMode()
    {
        // Example: Set character to normal mode
        Debug.Log("Character is in normal mode");
        // Add your normal mode logic here
        isInCombatMode = false;
    }
}
