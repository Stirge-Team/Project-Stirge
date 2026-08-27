using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;

    [NameOverride("World Position", 0)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Movement Goals/Move To World Position", fileName = "New Serialized Move To World Position Goal", order = 452)]
    public class SerializedMoveToWorldPositionGoal : SerializedMovementGoal<MoveToWorldPositionGoal, Vector3>
    {

    }
}
