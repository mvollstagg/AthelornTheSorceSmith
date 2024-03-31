using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
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
            Debug.Log("Character entered combat zone");
            // Switch to combat mode
            isInCombatMode = true;
            EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_LOCOMOTION_MODE_CHANGED, this, new OnCharacterLocomotionChangedEventArgs
            {
                LocomotionMode = LocomotionModeType.Combat
            });
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the character exits the combat zone
        if (other.CompareTag(combatZoneTag))
        {
            Debug.Log("Character exited combat zone");
            // Switch to normal mode
            isInCombatMode = true;
            EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_LOCOMOTION_MODE_CHANGED, this, new OnCharacterLocomotionChangedEventArgs
            {
                LocomotionMode = LocomotionModeType.Idle
            });
        }
    }
}
