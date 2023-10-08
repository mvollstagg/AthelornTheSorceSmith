using System;
using Scripts.Entities.Enum;

namespace Scripts.Entities.Class
{
    public static class GameEvents
    {
        public static string ON_MOUSE_ENTER_INTERACTABLE { get; private set; } = "OnMouseEnterInteractable";
        public static string ON_CAMERA_ROTATE_MOUSE { get; private set; } = "OnCameraRotateMouse";
        public static string ON_CAMERA_ROTATE_MOUSE_CANCELED { get; private set; } = "OnCameraRotateCanceledMouse";
        public static string ON_CAMERA_ROTATE_GAMEPAD { get; private set; } = "OnCameraRotateGamepad";
        public static string ON_CAMERA_ROTATE_GAMEPAD_CANCELED { get; private set; } = "OnCameraRotateCanceledGamepad";
        public static string ON_PLAY_SFX { get; private set; } = "OnPlaySoundEffects";
        public static string ON_CHARACTER_STAT_INFO_CHANGED { get; private set; } = "OnCharacterInfoChanged";
        public static string ON_CHARACTER_EQUIPPED_OR_UNEQUIPPED_ITEM { get; private set; } = "OnCharacterEquippedOrUnequippedItem";
        public static string ON_SAVEGAMEOBJECT_INITIALIZED { get; private set; } = "OnSaveGameObjectInitialized";
    }
}