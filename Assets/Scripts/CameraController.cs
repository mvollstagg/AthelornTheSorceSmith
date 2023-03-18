using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;

    public float rotationSpeed = 1f;


    void Update()
    {
        // TODO: Change transform rotation
        if (Input.GetKey(KeyCode.Q))
        {
            cameraTarget.RotateAround(cameraTarget.position, Vector3.up, rotationSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            cameraTarget.RotateAround(cameraTarget.position, Vector3.down, rotationSpeed);
        }
       
    }
}
