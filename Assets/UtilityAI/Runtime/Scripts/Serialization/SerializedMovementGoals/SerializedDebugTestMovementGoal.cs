using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    [NameOverride("Score", 0), NameOverride("Message", 1), NameOverride("Use Cosine instead of Sine", 2)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Movement Goals/Debug Test", fileName = "New Debug Test Movement Goal", order = 452)]
    public class SerializedDebugTestMovementGoal : SerializedMovementGoal<DebugTestMovementGoal, float, string, bool>
    {

    }
}
