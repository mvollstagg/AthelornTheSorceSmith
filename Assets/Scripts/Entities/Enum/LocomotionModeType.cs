using System.ComponentModel;

namespace Scripts.Entities.Enum
{
    public enum LocomotionModeType : int
    {
        [Description("Idle")]
        Idle = 10,
        [Description("Combat")]
        Combat = 20,
    }
}