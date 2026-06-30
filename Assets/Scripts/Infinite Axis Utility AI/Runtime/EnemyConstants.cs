using UnityEngine;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    public static class EnemyConstants
    {
        // Component Names
        public static BlackboardPropertyName UtilityEnemyName = new(nameof(UtilityEnemy));
        public static BlackboardPropertyName RigidbodyName = new(nameof(Rigidbody));

        // Property Names
        public static BlackboardPropertyName TargetName = new("m_target");

        public const string IsGrounded = "IsGrounded";
        public static BlackboardPropertyName IsGroundedName = new(IsGrounded);

        public const string GetPosition = "GetPosition";
        public static BlackboardPropertyName GetPositionName = new(GetPosition);

        public const string GetEulerRotation = "GetEulerRotation";
        public static BlackboardPropertyName GetEulerRotationName = new(GetEulerRotation);

        public const string GetSpeed = "GetSpeed";
        public static BlackboardPropertyName GetSpeedName = new(GetSpeed);
    }
}
