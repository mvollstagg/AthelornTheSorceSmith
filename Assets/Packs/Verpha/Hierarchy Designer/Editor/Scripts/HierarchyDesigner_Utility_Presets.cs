#if UNITY_EDITOR
using System.Collections.Generic;

namespace Verpha.HierarchyDesigner
{
    public static class HierarchyDesigner_Utility_Presets
    {
        private static List<HierarchyDesigner_Info_Presets> presets;
        public static List<HierarchyDesigner_Info_Presets> Presets
        {
            get
            {
                if (presets == null) { InitializePresets(); }
                return presets;
            }
        }

        private static void InitializePresets()
        {
            presets = new List<HierarchyDesigner_Info_Presets>
            {
                HierarchyDesigner_Info_Presets.AzureDreamscapePreset(),
                HierarchyDesigner_Info_Presets.BlackAndGoldPreset(),
                HierarchyDesigner_Info_Presets.BlackAndWhitePreset(),
                HierarchyDesigner_Info_Presets.BloodyMaryPreset(),
                HierarchyDesigner_Info_Presets.DeepOceanPreset(),
                HierarchyDesigner_Info_Presets.MinimalBlackPreset(),
                HierarchyDesigner_Info_Presets.MinimalWhitePreset(),
                HierarchyDesigner_Info_Presets.NaturePreset(),
                HierarchyDesigner_Info_Presets.NavyBlueLightPreset(),
                HierarchyDesigner_Info_Presets.OldSchoolPreset(),
                HierarchyDesigner_Info_Presets.PrettyPinkPreset(),
                HierarchyDesigner_Info_Presets.RedDawnPreset(),
                HierarchyDesigner_Info_Presets.SunflowerPreset(),
            };
        }

        public static string[] GetPresetNames()
        {
            return Presets.ConvertAll(p => p.presetName).ToArray();
        }

        public static void ApplyPresetToFolders(HierarchyDesigner_Info_Presets preset)
        {
            foreach (HierarchyDesigner_Info_Folder folder in HierarchyFolderWindow.folders.Values)
            {
                folder.FolderColor = preset.folderColor;
                folder.ImageType = preset.folderImageType;
            }

            HierarchyDesigner_Visual_Folder.UpdateFolderVisuals();
            HierarchyFolderWindow.SaveFolders();
        }

        public static void ApplyPresetToSeparators(HierarchyDesigner_Info_Presets preset)
        {
            foreach (HierarchyDesigner_Info_Separator separator in HierarchySeparatorWindow.separators.Values)
            {
                separator.TextColor = preset.separatorTextColor;
                separator.BackgroundColor = preset.separatorBackgroundColor;
                separator.FontStyle = preset.separatorFontStyle;
                separator.FontSize = preset.separatorFontSize;
                separator.TextAlignment = preset.separatorTextAlignment;
                separator.ImageType = preset.separatorBackgroundImageType;
            }

            HierarchyDesigner_Visual_Separator.UpdateSeparatorVisuals();
            HierarchySeparatorWindow.SaveSeparators();
        }

        public static void ApplyPresetToTagLayer(HierarchyDesigner_Info_Presets preset)
        {
            HierarchyDesigner_Manager_Settings.TagLayerTextColor = preset.tagLayerTextColor;
            HierarchyDesigner_Manager_Settings.TagLayerFontStyle = preset.tagLayerFontStyle;
            HierarchyDesigner_Manager_Settings.TagLayerFontSize = preset.tagLayerFontSize;
            HierarchyDesigner_Visual_GameObject.RecalculateTagAndLayerSizes();
        }

        public static void ApplyPresetToTree(HierarchyDesigner_Info_Presets preset)
        {
            HierarchyDesigner_Visual_GameObject.UpdateTreeColorCache();
            HierarchyDesigner_Manager_Settings.TreeColor = preset.treeColor;
        }
    }
}
#endif