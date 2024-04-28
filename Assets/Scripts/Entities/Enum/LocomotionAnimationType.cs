using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum LocomotionAnimationType
    {
        [Description("Forward Left")]
        ForwardLeft,
        [Description("Forward")]
        Forward,
        [Description("Forward Right")]
        ForwardRight,
        [Description("Left")]
        Left,
        [Description("Right")]
        Right,
        [Description("Backward Right")]
        BackwardRight,
        [Description("Backward")]
        Backward,
        [Description("Backward Left")]
        BackwardLeft,
        [Description("Idle")]
        Idle
    }
}