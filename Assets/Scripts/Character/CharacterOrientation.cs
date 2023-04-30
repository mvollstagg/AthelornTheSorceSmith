using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

public class CharacterOrientation : Singleton<CharacterOrientation>
{
    #region Public Variables
    [Tooltip("Sprint speed of the character in m/s")]
    public float TurnSpeed = 5f;
    #endregion

    #region Close Public Variables
    #endregion

    #region Private Variables
    #endregion

    void Update()
    {
        if(Character.Instance.IsCurrentDeviceMouse)
            RotatePlayer();
    }
    
    private void RotatePlayer()
    {
        if(CharacterRotateCamera.Instance.cursorLocked)
        {

        }
        else
        {
            // Raycast from the camera to the point on the ground where the mouse is pointing
            Ray ray = Character.Instance._mainCamera.ScreenPointToRay(CursorManager.Instance._mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && InputManager.Instance.move == Vector2.zero )
            {
                // Calculate the angle between the character and the point on the ground
                Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                Vector3 direction = targetPosition - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                float angle = rotation.eulerAngles.y;

                // Rotate the character towards the point on the ground, only rotating around the y-axis
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, angle, 0f), TurnSpeed * Time.deltaTime);
            }
        }
    }
}
