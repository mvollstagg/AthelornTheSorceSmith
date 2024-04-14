#if UNITY_EDITOR
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    [System.Serializable]
    public class HierarchyDesigner_Info_Folder
    {
        #region Properties
        [SerializeField] private string name = "Folder";
        [SerializeField] private Color folderColor = Color.white;
        [SerializeField] private FolderImageType folderImageType = FolderImageType.Default;
        #endregion

        public string Name { get => name; set => name = value; }
        public Color FolderColor { get => folderColor; set => folderColor = value; }
        public FolderImageType ImageType { get => folderImageType; set => folderImageType = value; }

        public enum FolderImageType
        {
            Default,
            DefaultOutline,
            DefaultOutline2X
        }

        public HierarchyDesigner_Info_Folder() { }

        public HierarchyDesigner_Info_Folder(string name, Color folderColor, FolderImageType folderImageType)
        {
            this.name = name;
            this.folderColor = folderColor;
            this.folderImageType = folderImageType;
        }
    }
}
#endif
