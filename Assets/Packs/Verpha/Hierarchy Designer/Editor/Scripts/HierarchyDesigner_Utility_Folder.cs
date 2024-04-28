#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    public class HierarchyFolderWindow : EditorWindow
    {
        #region OnGUI Properties
        private Vector2 scrollPositionOuter;
        private Vector2 scrollPositionInner;
        private static bool hasModifiedChanges = false;
        private GUIStyle customSettingsStyle;
        private GUIStyle customSettingsStyleSecondary;
        private GUIStyle headerLabelStyle;
        private GUIStyle contentLabelStyle;
        #endregion
        #region Folder Properties
        private string newFolderName = "";
        private Color newFolderIconColor = Color.white;
        private HierarchyDesigner_Info_Folder.FolderImageType newFolderImageType = HierarchyDesigner_Info_Folder.FolderImageType.Default;
        private const string FolderPrefKey = "HierarchyFolders";
        public static Dictionary<string, HierarchyDesigner_Info_Folder> folders = new Dictionary<string, HierarchyDesigner_Info_Folder>();
        #endregion
        #region Global Fields
        private Color tempGlobalFolderIconColor = Color.white;
        private HierarchyDesigner_Info_Folder.FolderImageType tempGlobalFolderImageType = HierarchyDesigner_Info_Folder.FolderImageType.Default;
        #endregion

        [MenuItem("Hierarchy Designer/Hierarchy Folder/Hierarchy Folder Manager")]
        private static void OpenWindow()
        {
            LoadFolders();
            HierarchyFolderWindow window = GetWindow<HierarchyFolderWindow>("Hierarchy Folder Manager");
            window.minSize = new Vector2(300, 150);
        }
        private void InitializeStyles()
        {
            customSettingsStyle = HierarchyDesigner_Info_OnGUI.CreateCustomStyle();
            customSettingsStyleSecondary = HierarchyDesigner_Info_OnGUI.CreateCustomStyle(true);
            headerLabelStyle = HierarchyDesigner_Info_OnGUI.HeaderLabelStyle;
            contentLabelStyle = HierarchyDesigner_Info_OnGUI.ContentLabelStyle;
        }

        private void OnGUI()
        {
            InitializeStyles();
            EditorGUILayout.BeginVertical(customSettingsStyle);

            GUILayout.Space(4);
            EditorGUILayout.LabelField("Folders Manager", headerLabelStyle);
            GUILayout.Space(8);

            scrollPositionOuter = EditorGUILayout.BeginScrollView(scrollPositionOuter, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Folder Creation Fields
            using (new HierarchyDesigner_Info_OnGUI.LabelWidth(60))
            {
                newFolderName = EditorGUILayout.TextField("Name", newFolderName);
                newFolderIconColor = EditorGUILayout.ColorField("Color", newFolderIconColor);
                newFolderImageType = (HierarchyDesigner_Info_Folder.FolderImageType)EditorGUILayout.EnumPopup("Image", newFolderImageType);
            }
            #endregion

            #region Add Folder
            GUILayout.Space(10);
            if (GUILayout.Button("Add Folder", GUILayout.Height(25)))
            {
                if (IsFolderNameValid(newFolderName))
                {
                    HierarchyDesigner_Info_Folder newFolder = new HierarchyDesigner_Info_Folder(newFolderName, newFolderIconColor, newFolderImageType);
                    folders[newFolderName] = newFolder;
                    newFolderName = "";
                    GUI.FocusControl(null);
                    hasModifiedChanges = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Folder Name", "Folder name is either duplicate or invalid.", "OK");
                }
            }
            GUILayout.Space(5);
            #endregion

            #region Folders List
            float maxWidth = HierarchyDesigner_Info_OnGUI.CalculateMaxLabelWidth(folders.Keys);

            if (folders.Count > 0)
            {
                EditorGUILayout.BeginVertical(customSettingsStyleSecondary);

                GUILayout.Space(2);
                EditorGUILayout.LabelField("Folders’ List", contentLabelStyle);
                GUILayout.Space(10);

                scrollPositionInner = EditorGUILayout.BeginScrollView(scrollPositionInner, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                foreach (var folderEntry in folders)
                {
                    string key = folderEntry.Key;
                    HierarchyDesigner_Info_Folder folder = folderEntry.Value;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField(folder.Name, GUILayout.Width(maxWidth));
                    folder.FolderColor = EditorGUILayout.ColorField(folder.FolderColor, GUILayout.MinWidth(200), GUILayout.ExpandWidth(true));
                    folder.ImageType = (HierarchyDesigner_Info_Folder.FolderImageType)EditorGUILayout.EnumPopup(folder.ImageType, GUILayout.Width(125));
                    if (EditorGUI.EndChangeCheck())
                    {
                        hasModifiedChanges = true;
                    }

                    if (GUILayout.Button("Create", GUILayout.Width(60)))
                    {
                        HierarchyDesigner_Utility_Folder.CreateFolder(folder);
                    }
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        folders.Remove(key);
                        hasModifiedChanges = true;
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(5);
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                #region Global Fields
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Global Fields:", contentLabelStyle, GUILayout.Width(95));
                GUILayout.Space(10);

                #region Changes Check
                EditorGUI.BeginChangeCheck();
                tempGlobalFolderIconColor = EditorGUILayout.ColorField(tempGlobalFolderIconColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (HierarchyDesigner_Info_Folder folder in folders.Values)
                    {
                        folder.FolderColor = tempGlobalFolderIconColor;
                    }
                    hasModifiedChanges = true;
                }
                
                EditorGUI.BeginChangeCheck();
                tempGlobalFolderImageType = (HierarchyDesigner_Info_Folder.FolderImageType)EditorGUILayout.EnumPopup(tempGlobalFolderImageType, GUILayout.Width(258));
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (HierarchyDesigner_Info_Folder folder in folders.Values)
                    {
                        folder.ImageType = tempGlobalFolderImageType;
                    }
                    hasModifiedChanges = true;
                }
                #endregion

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
                #endregion

                GUILayout.Space(2);
                if (GUILayout.Button("Update Folders", GUILayout.Height(30)))
                {
                    HierarchyDesigner_Visual_Folder.UpdateFolderVisuals();
                }
                GUILayout.Space(2);
                if (GUILayout.Button("Save Folders", GUILayout.Height(30)))
                {
                    SaveFolders();
                    HierarchyDesigner_Visual_Folder.UpdateFolderVisuals();
                }
                GUILayout.Space(5);
            }
            #endregion

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private bool IsFolderNameValid(string folderName)
        {
            return !string.IsNullOrEmpty(folderName) && !folders.ContainsKey(folderName);
        }

        public static void SaveFolders()
        {
            List<HierarchyDesigner_Info_Folder> folderList = folders.Values.ToList();
            string json = JsonUtility.ToJson(new Serialization<HierarchyDesigner_Info_Folder>(folderList), true);

            string folderPath = Path.Combine(Application.dataPath, "Settings/Hierarchy");
            string filePath = Path.Combine(folderPath, "folders.json");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllText(filePath, json);
            hasModifiedChanges = false;

            Debug.Log("Folders saved to JSON: " + filePath);
        }

        public static void LoadFolders()
        {
            string folderPath = Path.Combine(Application.dataPath, "Settings/Hierarchy");
            string filePath = Path.Combine(folderPath, "folders.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Serialization<HierarchyDesigner_Info_Folder> data = JsonUtility.FromJson<Serialization<HierarchyDesigner_Info_Folder>>(json);
                folders.Clear();

                foreach (var folder in data.items)
                {
                    folders[folder.Name] = folder;
                }
                hasModifiedChanges = false;
            }
            else
            {
                Debug.Log("No folder data file found. Ensure that folders are saved before loading.");
            }
        }

        private void OnDestroy()
        {
            if (hasModifiedChanges)
            {
                bool shouldSave = EditorUtility.DisplayDialog("Folder(s) Have Been Modified",
                    "Do you want to save the changes you made in the folders?",
                    "Save", "Don't Save");

                if (shouldSave)
                {
                    SaveFolders();
                }
            }
            hasModifiedChanges = false;
        }
    }

    public class HierarchyDesigner_Utility_Folder
    {
        #region Default Folder
        [MenuItem("Hierarchy Designer/Hierarchy Folder/Create Default Folder", false, 2)]
        private static void CreateDefaultFolder()
        {
            CreateFolderObject("New Folder", HierarchyDesigner_Info_Folder.FolderImageType.Default);
        }

        private static void CreateFolderObject(string folderName, HierarchyDesigner_Info_Folder.FolderImageType folderImageType)
        {
            GameObject folder = new GameObject(folderName);
            folder.AddComponent<HierarchyDesignerFolder>();

            Texture2D folderIcon = HierarchyDesigner_Manager_Folder.GetFolderIcon(folderImageType);
            if (folderIcon != null)
            {
                EditorGUIUtility.SetIconForObject(folder, folderIcon);
            }

            Undo.RegisterCreatedObjectUndo(folder, $"Create {folderName}");
            EditorGUIUtility.PingObject(folder);
        }
        #endregion

        [MenuItem("Hierarchy Designer/Hierarchy Folder/Create All Folders", false, 1)]
        private static void CreateAllFoldersFromList()
        {
            foreach (HierarchyDesigner_Info_Folder folderInfo in HierarchyFolderWindow.folders.Values)
            {
                CreateFolder(folderInfo);
            }
        }

        [MenuItem("Hierarchy Designer/Hierarchy Folder/Create Missing Folders", false, 2)]
        private static void CreateMissingFolders()
        {
            foreach (HierarchyDesigner_Info_Folder folderInfo in HierarchyFolderWindow.folders.Values)
            {
                if (!FolderExists(folderInfo.Name))
                {
                    CreateFolder(folderInfo);
                }
            }
        }

        public static void CreateFolder(HierarchyDesigner_Info_Folder folderInfo)
        {
            GameObject folder = new GameObject(folderInfo.Name);

            folder.AddComponent<HierarchyDesignerFolder>();

            Undo.RegisterCreatedObjectUndo(folder, $"Create {folderInfo.Name}");

            Texture2D inspectorIcon = HierarchyDesigner_Manager_Folder.InspectorFolderIcon;
            if (inspectorIcon != null)
            {
                EditorGUIUtility.SetIconForObject(folder, inspectorIcon);
            }

            EditorGUIUtility.PingObject(folder);
        }

        private static bool FolderExists(string folderName)
        {
            HierarchyDesignerFolder[] allFolders = UnityEngine.Object.FindObjectsOfType<HierarchyDesignerFolder>(includeInactive: true);
            foreach (HierarchyDesignerFolder folder in allFolders)
            {
                if (folder.gameObject.name.Equals(folderName, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
#endif