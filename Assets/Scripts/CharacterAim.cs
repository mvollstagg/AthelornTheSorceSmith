using UnityEngine;

public class CharacterAim : MonoBehaviour
{
    public float turnSpeed = 10f; // The speed at which the character turns
    private Camera mainCamera;
    [SerializeField] private float rayLength = 100f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Raycast from the camera to the point on the ground where the mouse is pointing
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Calculate the angle between the character and the point on the ground
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 direction = targetPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            float angle = rotation.eulerAngles.y;

            // Rotate the character towards the point on the ground, only rotating around the y-axis
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, angle, 0f), turnSpeed * Time.deltaTime);
        }

        // Debug draw line to show the direction the character is facing
        Debug.DrawRay(transform.position, transform.forward * rayLength, Color.green);
    }
}
