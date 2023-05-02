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
    }
}