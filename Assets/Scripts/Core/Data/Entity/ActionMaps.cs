using System;
using Scripts.Entities.Enum;

namespace Scripts.Entities.Class
{
    public static class ActionMaps
    {
        public static string CHARACTER { get; private set; } = "Character";
        public static string UI { get; private set; } = "UI";
        public static string INTERACTION { get; private set; } = "Interaction";
        public static string CAMERA { get; private set; } = "Camera";
        public static string INGAMEMENU { get; private set; } = "InGameMenu";
        public static string TRANSITION { get; private set; } = "Transition";
    }
}