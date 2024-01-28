using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum LocomotionModeType : int
    {
        [Description("Idle")]
        Idle = 0,
        [Description("Combat")]
        Combat = 1,
    }
}