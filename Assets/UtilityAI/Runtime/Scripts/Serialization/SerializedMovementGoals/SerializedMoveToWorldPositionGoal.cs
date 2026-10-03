using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    using Serialization;

    [CreateAssetMenu(menuName = "Utility AI/Serialized Movement Goals/Move To World Position", fileName = "New Move To World Position Goal", order = 452)]
    [NameOverride("World Position", 0)]
    public class SerializedMoveToWorldPositionGoal : SerializedMovementGoal<MoveToWorldPositionGoal, Vector3> { }
}
