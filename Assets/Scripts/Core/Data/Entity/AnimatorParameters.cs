using System;
using Scripts.Entities.Enum;

namespace Scripts.Entities.Class
{
    public static class AnimatorParameters
    {
        public static string LOCOMOTIOM_MODE { get; private set; } = "LocomotionMode";
        public static string IDLE { get; private set; } = "Idle";
        public static string MOVING { get; private set; } = "Moving";
        public static string RUNNING { get; private set; } = "Running";
        public static string JUMP { get; private set; } = "Jump";
        public static string GROUNDED { get; private set; } = "Grounded";
        public static string FREE_FALL { get; private set; } = "FreeFall";
        public static string SPEED { get; private set; } = "Speed";
        public static string EQUIPPED_WEAPON_ID { get; private set; } = "EquippedWeaponId";
        public static string WEAPON_EQUIPPED { get; private set; } = "WeaponEquipped";
        public static string REMAPPED_MOVE_INPUT_X { get; private set; } = "RemappedMoveInputX";
        public static string REMAPPED_MOVE_INPUT_Y { get; private set; } = "RemappedMoveInputY";
        public static string MOTION_SPEED { get; private set; } = "MotionSpeed";
        public static string TRIGGER { get; private set; } = "Trigger";
        public static string ACTION { get; private set; } = "Action";
        public static string ATTACK { get; private set; } = "Attack";
        public static string CASTING_SPELL_ID { get; private set; } = "CastingSpellId";
        public static string ANIMATION_BLEND { get; private set; } = "AnimationBlend";
    }
}