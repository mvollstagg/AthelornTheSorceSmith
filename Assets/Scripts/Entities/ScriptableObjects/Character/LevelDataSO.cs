using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Scripts.Entities.Class;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/Character/LevelDataSO", order = 2)]
public class LevelDataSO : ScriptableObject
{
    public List<ExperienceLevel> levels = new List<ExperienceLevel>();

    public int GetRequiredExperience(int level)
    {
        // Assuming your LevelDataSO has a List<ExperienceLevel> called 'levels'
        if (level > 0 && level <= levels.Count)
        {
            return levels[level - 1].requiredExperience;
        }
        else
        {
            // Return a default value or handle the case when the level is out of range
            return 0;
        }
    }

    // Custom editor script to add levels in the Inspector
    #if UNITY_EDITOR
    [CustomEditor(typeof(LevelDataSO))]
    public class LevelDataSOEditor : Editor
    {
        private SerializedProperty levelsProperty;

        private void OnEnable()
        {
            levelsProperty = serializedObject.FindProperty("levels");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            LevelDataSO levelData = (LevelDataSO)target;

            GUILayout.Space(10f);

            if (GUILayout.Button("Add Level"))
            {
                int nextLevel = 1;
                int requiredExp = 0;
                if (levelData.levels.Count > 0)
                {
                    nextLevel = levelData.levels[levelData.levels.Count - 1].Level + 1;
                    requiredExp = Mathf.Max(levelData.levels[levelData.levels.Count - 1].requiredExperience * 2, 100);
                }
                else
                {
                    requiredExp = 100; // Set required experience to 100 for the first level
                }

                levelData.levels.Add(new ExperienceLevel(nextLevel, requiredExp));
            }

            EditorGUI.BeginDisabledGroup(true); // Disable editing of level numbers

            for (int i = 0; i < levelsProperty.arraySize; i++)
            {
                SerializedProperty levelElement = levelsProperty.GetArrayElementAtIndex(i);
                SerializedProperty levelNumber = levelElement.FindPropertyRelative("level");
                SerializedProperty requiredExperience = levelElement.FindPropertyRelative("requiredExperience");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level " + levelNumber.intValue.ToString(), GUILayout.Width(80f));
                EditorGUILayout.PropertyField(requiredExperience, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                // Ensure level number matches the index + 1
                levelNumber.intValue = i + 1;

                // Ensure minimum required experience is +100
                requiredExperience.intValue = Mathf.Max(requiredExperience.intValue, 100);
            }

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}