using UnityEngine;

namespace Stirge.UtilityAI.Actions
{
    using Serialization;

    [NameOverride("Check Mask", 0), NameOverride("Check Range", 1)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Actions/Find Target", fileName = "New Find Target Action", order = 452)]
    public class SerializedFindTargetAction : SerializedAction<FindTargetAction, LayerMask> { }
}
