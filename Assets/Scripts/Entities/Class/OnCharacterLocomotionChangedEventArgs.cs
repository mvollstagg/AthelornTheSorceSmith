using System;
using Scripts.Entities.Enum;

namespace Scripts.Entities.Class
{
    public class OnCharacterLocomotionChangedEventArgs : EventArgs
    {
        public LocomotionModeType LocomotionMode;
    }
}