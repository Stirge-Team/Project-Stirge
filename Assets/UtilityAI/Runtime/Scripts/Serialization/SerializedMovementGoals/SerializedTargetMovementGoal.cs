using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    using Serialization;

    [NameOverride("Max Score", 0)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Movement Goals/Target", fileName = "New Target Movement Goal", order = 452)]
    public class SerializedTargetMovementGoal : SerializedMovementGoal<TargetMovementGoal, float>
    {

    }
}
