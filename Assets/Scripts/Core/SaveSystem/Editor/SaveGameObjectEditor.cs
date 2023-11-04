using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using Scripts.Core.SaveSystem.Entities;
using System.Linq;

[CustomEditor(typeof(SaveGameObject))]
public class SaveGameObjectEditor : Editor
{
    private Dictionary<string, bool> componentFoldouts = new Dictionary<string, bool>();
    private Dictionary<FieldInfo, bool> fieldSelections = new Dictionary<FieldInfo, bool>();

    public override void OnInspectorGUI()
    {
        SaveGameObject saveGameObject = (SaveGameObject)target;

        EditorGUI.BeginChangeCheck();

        // Iterate over all components on the GameObject
        foreach (Component component in saveGameObject.gameObject.GetComponents<Component>())
        {
            if (component == null) continue;

            string componentName = component.GetType().Name;
            bool foldout = componentFoldouts.ContainsKey(componentName) ? componentFoldouts[componentName] : false;

            // Begin a box to highlight the component's header
            GUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            // Use a default icon for custom scripts
            GUIContent iconContent;
            if (component is SaveGameObject)
            {
                iconContent = EditorGUIUtility.IconContent("cs Script Icon");
            }
            else
            {
                iconContent = EditorGUIUtility.IconContent(component.GetType().Name + " Icon");
            }

            if (iconContent.image != null)
            {
                GUILayout.Label(iconContent, GUILayout.Width(20), GUILayout.Height(20));
            }

            // Use a label as a button for the foldout mechanism
            if (GUILayout.Button(componentName, EditorStyles.label))
            {
                foldout = !foldout;

                // Only one component can be expanded at a time
                foreach (var key in componentFoldouts.Keys.ToList())
                {
                    componentFoldouts[key] = false;
                }
                componentFoldouts[componentName] = foldout;
            }

            // Checkbox to include the component in the save
            bool saveComponent = EditorGUILayout.Toggle(saveGameObject.componentsToSave.Exists(x => x.componentName == componentName && x.saveComponent), GUILayout.Width(15));

            EditorGUILayout.EndHorizontal();

            if (foldout)
            {
                DrawComponentFields(component);
            }

            GUILayout.EndVertical();

            // Update the saveComponent field based on the checkbox
            UpdateComponentData(saveGameObject, componentName, saveComponent);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(saveGameObject, "SaveGameObject Change");
            EditorUtility.SetDirty(saveGameObject);
        }
    }

    private void DrawSeparator()
    {
        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        EditorGUILayout.Space();
    }

    private void UpdateComponentData(SaveGameObject saveGameObject, string componentName, bool saveComponent)
    {
        ComponentData compData = saveGameObject.componentsToSave.Find(x => x.componentName == componentName);
        if (compData != null)
        {
            compData.saveComponent = saveComponent;
        }
        else if (saveComponent)
        {
            saveGameObject.componentsToSave.Add(new ComponentData { componentName = componentName, saveComponent = true });
        }
        else
        {
            saveGameObject.componentsToSave.RemoveAll(x => x.componentName == componentName);
        }
    }

    private void DrawComponentFields(Component component)
    {
        SaveGameObject saveGameObject = (SaveGameObject)target;
        string componentName = component.GetType().Name;
        ComponentData compData = saveGameObject.componentsToSave.Find(x => x.componentName == componentName);

        if (compData == null)
        {
            compData = new ComponentData { componentName = componentName };
            saveGameObject.componentsToSave.Add(compData);
        }

        // Checkboxes for fields
        EditorGUILayout.LabelField("Fields", EditorStyles.boldLabel);
        var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            if (field.IsNotSerialized) continue;
            bool isSelected = compData.fieldsToSave.Contains(field.Name);
            bool newIsSelected = EditorGUILayout.ToggleLeft(field.Name, isSelected, GUILayout.ExpandWidth(false));
            if (newIsSelected != isSelected)
            {
                if (newIsSelected)
                    compData.fieldsToSave.Add(field.Name);
                else
                    compData.fieldsToSave.Remove(field.Name);
            }
        }

        // Add space between fields and properties
        EditorGUILayout.Space();
        DrawSeparator();

        // Checkboxes for properties
        EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
        var properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (PropertyInfo property in properties)
        {
            if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length > 0) continue;
            bool isSelected = compData.propertiesToSave.Contains(property.Name);
            bool newIsSelected = EditorGUILayout.ToggleLeft(property.Name, isSelected, GUILayout.ExpandWidth(false));
            if (newIsSelected != isSelected)
            {
                if (newIsSelected)
                    compData.propertiesToSave.Add(property.Name);
                else
                    compData.propertiesToSave.Remove(property.Name);
            }
        }
    }

}
