using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject CinemachineCameraTarget;
    private GameInput _input;
    [SerializeField] private Transform cameraFollowTarget;
    public float rotationSpeed = 1f;
	private PlayerInputActions playerInputActions;
    private bool rotateLeft, rotateRight;

	void Awake()
	{
        _input = GameInput.Instance;
		playerInputActions = new();
        playerInputActions.Camera.Enable();

		playerInputActions.Camera.RotateRight.performed += _ => rotateRight = !rotateRight;
		playerInputActions.Camera.RotateLeft.performed += _ => rotateLeft = !rotateLeft;

        CinemachineCameraTarget.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = cameraFollowTarget;
	}

    void OnDestroy()
    {
        playerInputActions.Dispose();
    }
    
    void LateUpdate()
    {
        if (rotateLeft)
        {
            CinemachineCameraTarget.transform.RotateAround(cameraFollowTarget.position, Vector3.up, rotationSpeed);
        }
        if (rotateRight)
        {
            CinemachineCameraTarget.transform.RotateAround(cameraFollowTarget.position, Vector3.down, rotationSpeed);
        }       
    }
}
