using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CharacterDirectionMarker : MonoBehaviour
{
    public Transform player; // The player's transform
    public Transform marker; // The transform of the marker object

    void Update()
    {
        // Get the direction the player is facing
        Vector3 playerDirection = player.forward;

        // Orient the marker to face the same direction as the player
        marker.rotation = Quaternion.LookRotation(playerDirection);
    }
}
