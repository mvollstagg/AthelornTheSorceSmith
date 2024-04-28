using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTestScript : MonoBehaviour
{

    public float speed;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;

    private Vector3 offset;
    private Rigidbody rb;
    private Vector3 targetPosition;

    void Start()
    {
        // Calculate direction towards mouse position
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            targetPosition = ray.GetPoint(position) + new Vector3(0f, 1f, 0f);
        }

        rb = GetComponent<Rigidbody>();

        if (muzzlePrefab != null)
        {
            // Instantiate muzzle VFX
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        // Play the sound attached AudioSource on the vfxInstance
        AudioSource audioSource = this.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        // Move towards the target (mouse position)
        if (speed != 0 && rb != null)
            rb.position += (targetPosition - transform.position).normalized * (speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision co)
    {
        rb.useGravity = true;
        rb.drag = 0.5f;
        ContactPoint contact = co.contacts[0];
        rb.AddForce(Vector3.Reflect((contact.point).normalized, contact.normal), ForceMode.Impulse);
        Destroy(this);
    }

    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
