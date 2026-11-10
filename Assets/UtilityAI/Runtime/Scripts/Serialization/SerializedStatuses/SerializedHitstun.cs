using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    [NameOverride("Animation State Name", "The animation to play while stunned.", 0)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Statuses/Hitstun", fileName = "New Hitstun")]
    public class SerializedHitstun : SerializedStatus<Hitstun, string> { }
}
