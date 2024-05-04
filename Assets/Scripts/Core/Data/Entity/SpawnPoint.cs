using System.Collections;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    public class SpawnPoint : MonoBehaviour
    {
        // Settings for this spawn point
        public GameObject objectPrefab;
        public int spawnCount;
        public float spawnInterval;
        public SpawnAreaType areaType;
        public float spawnDistance = 5f; // The distance within which objects will be spawned
        private int spawnedCount;
        private Coroutine respawnCoroutine;
        

        private void Start()
        {
            // Add this spawn point to the SpawnManager's list when it is created
            SpawnManager.Instance.AddSpawnPoint(this);
            if (objectPrefab == null)
            {
                Debug.LogWarning("SpawnPoint deactivating because objectPrefab is null", this);
                gameObject.SetActive(false);
                return;
            }
            StartSpawning();
        }

        public void StartSpawning()
        {
            spawnedCount = 0;
            respawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        public void StopSpawning()
        {
            if (respawnCoroutine != null)
            {
                StopCoroutine(respawnCoroutine);
            }
        }


        private IEnumerator SpawnRoutine()
        {
            while (spawnedCount < spawnCount)
            {
                Vector3 spawnPosition = CalculateSpawnPosition();
                Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
                spawnedCount++;
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private Vector3 CalculateSpawnPosition()
        {
            if (areaType == SpawnAreaType.Box)
            {
                // Generate random position within a box area
                Vector3 halfSize = Vector3.one * spawnDistance;
                Vector3 randomPosition = new Vector3(
                    Random.Range(transform.position.x - halfSize.x, transform.position.x + halfSize.x),
                    Random.Range(transform.position.y - halfSize.y, transform.position.y + halfSize.y),
                    Random.Range(transform.position.z - halfSize.z, transform.position.z + halfSize.z)
                );
                return randomPosition;
            }
            else if (areaType == SpawnAreaType.Circle)
            {
                // Generate random position within a circular area
                Vector2 randomCirclePoint = Random.insideUnitCircle * spawnDistance;
                Vector3 randomPosition = new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y) + transform.position;
                return randomPosition;
            }

            return transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the spawn area when the GameObject is selected

            Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Yellow color with 30% transparency
            Vector3 spawnAreaCenter = transform.position;
            float spawnAreaRadius = spawnDistance;

            if (areaType == SpawnAreaType.Circle)
            {
                Gizmos.DrawSphere(spawnAreaCenter, spawnAreaRadius);
            }
            else if (areaType == SpawnAreaType.Box)
            {
                Vector3 halfSize = Vector3.one * spawnAreaRadius;
                Gizmos.DrawCube(spawnAreaCenter, halfSize * 2);
            }
        }
    }
}
