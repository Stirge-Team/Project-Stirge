using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    public static class EnemyConstants
    {
        public static BlackboardPropertyName UtilityEnemyComponentName = new BlackboardPropertyName("UtilityEnemy");
        public const string IsGrounded = "IsGrounded";

        public const string GetPosition = "GetPosition";

        public const string GetEulerRotation = "GetEulerRotation";

        public const string GetSpeed = "GetSpeed";
    }
}
