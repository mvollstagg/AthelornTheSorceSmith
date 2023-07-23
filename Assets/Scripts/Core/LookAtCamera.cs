using System.Collections;
using Scripts.Entities.Enum;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private UICameraMode mode;
    void LateUpdate()
    {
        switch(mode)
        {
            case UICameraMode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case UICameraMode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case UICameraMode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case UICameraMode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }        
    }
}
