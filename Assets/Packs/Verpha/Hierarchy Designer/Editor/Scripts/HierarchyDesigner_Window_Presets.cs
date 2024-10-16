#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    public class HierarchyDesigner_Window_Presets : EditorWindow
    {
        #region OnGUI Properties
        private Vector2 scrollPositionOuter;
        private GUIStyle customSettingsStyle;
        private GUIStyle customSettingsStyleSecondary;
        private GUIStyle headerLabelStyle;
        #endregion
        #region Presets Properties
        private int selectedPresetIndex = 0;
        private string[] presetNames;
        private bool applyToFolders = true;
        private bool applyToSeparators = true;
        private bool applyToTagAndLayer = true;
        private bool applyToTree = true;
        #endregion

        [MenuItem("Hierarchy Designer/Hierarchy Helpers/Presets", false, 50)]
        private static void OpenWindow()
        {
            HierarchyDesigner_Window_Presets window = GetWindow<HierarchyDesigner_Window_Presets>("Hierarchy Presets");
            window.minSize = new Vector2(250, 125);
        }

        private void OnEnable()
        {
            presetNames = HierarchyDesigner_Utility_Presets.GetPresetNames();
        }

        private void InitializeStyles()
        {
            customSettingsStyle = HierarchyDesigner_Info_OnGUI.CreateCustomStyle();
            customSettingsStyleSecondary = HierarchyDesigner_Info_OnGUI.CreateCustomStyle(true);
            headerLabelStyle = HierarchyDesigner_Info_OnGUI.HeaderLabelStyle;
        }

        private void OnGUI()
        {
            InitializeStyles();
            EditorGUILayout.BeginVertical(customSettingsStyle);

            GUILayout.Space(4);
            EditorGUILayout.LabelField("Styling Preset", headerLabelStyle);
            GUILayout.Space(8);

            scrollPositionOuter = EditorGUILayout.BeginScrollView(scrollPositionOuter, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Presets Fields
            GUILayout.Label("Choose A Preset:", GUILayout.Width(200));
            GUILayout.Space(2);
            selectedPresetIndex = EditorGUILayout.Popup(selectedPresetIndex, presetNames);

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(customSettingsStyleSecondary);
            EditorGUILayout.LabelField("Apply Preset To:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            applyToFolders = EditorGUILayout.Toggle("Folders", applyToFolders);
            applyToSeparators = EditorGUILayout.Toggle("Separators", applyToSeparators);
            applyToTagAndLayer = EditorGUILayout.Toggle("Tag and Layer", applyToTagAndLayer);
            applyToTree = EditorGUILayout.Toggle("Hierarchy Tree", applyToTree);
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.Space(2);
            if (GUILayout.Button("Apply Preset", GUILayout.Height(30)))
            {
                ApplySelectedPreset();
            }
            GUILayout.Space(5);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ApplySelectedPreset()
        {
            if (selectedPresetIndex < 0 || selectedPresetIndex >= presetNames.Length) return;

            HierarchyDesigner_Info_Presets selectedPreset = HierarchyDesigner_Utility_Presets.Presets[selectedPresetIndex];

            string message = "Are you sure you want to override your current values for: ";
            List<string> changesList = new List<string>();
            if (applyToFolders) changesList.Add("Folders");
            if (applyToSeparators) changesList.Add("Separators");
            if (applyToTagAndLayer) changesList.Add("Tag and Layer");
            if (applyToTree) changesList.Add("Hierarchy Tree");
            message += string.Join(", ", changesList) + "?\n\n*If you select 'confirm' all values will be overridden and saved.*";

            if (EditorUtility.DisplayDialog("Confirm Preset Application", message, "Confirm", "Cancel"))
            {
                if (applyToFolders)
                {
                    HierarchyDesigner_Utility_Presets.ApplyPresetToFolders(selectedPreset);
                }
                if (applyToSeparators)
                {
                    HierarchyDesigner_Utility_Presets.ApplyPresetToSeparators(selectedPreset);
                }
                if (applyToTagAndLayer)
                {
                    HierarchyDesigner_Utility_Presets.ApplyPresetToTagLayer(selectedPreset);
                }
                if (applyToTree)
                {
                    HierarchyDesigner_Utility_Presets.ApplyPresetToTree(selectedPreset);
                }

                EditorApplication.RepaintHierarchyWindow();
            }
            else
            {
                return;
            }
        }
    }
}
#endif