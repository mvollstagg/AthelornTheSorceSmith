using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Class;
using UnityEditor;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    // List to store all the spawn points
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    // Method to add a new spawn point to the list
    public void AddSpawnPoint(SpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpawnManager))]
    public class SpawnManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Ensure serializedObject is updated
            serializedObject.Update();

            DrawDefaultInspector();

            SpawnManager spawnManager = (SpawnManager)target;

            GUILayout.Space(10f);

            if (GUILayout.Button("Add Spawn Point"))
            {
                CreateSpawnPointObject();
            }
        }

        private void CreateSpawnPointObject()
        {
            GameObject spawnPointsParent = GameObject.Find("Spawn Points");
            if (spawnPointsParent == null)
            {
                spawnPointsParent = new GameObject("Spawn Points");
            }

            GameObject newSpawnPointObject = new GameObject("SpawnPoint");
            newSpawnPointObject.transform.position = Vector3.zero;
            SpawnPoint newSpawnPoint = newSpawnPointObject.AddComponent<SpawnPoint>();

            // Make the new spawn point a child of the SpawnManager GameObject
            newSpawnPointObject.transform.SetParent(spawnPointsParent.transform);

            // Select the newly created spawn point in the hierarchy
            Selection.activeGameObject = newSpawnPointObject;
        }
    }
#endif
}
