using UnityEngine;

namespace Stirge.UtilityAI.Actions
{
    using Serialization;

    [NameOverride("Score", 0), NameOverride("Message", 1), NameOverride("Use Cosine instead of Sine", 2)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Actions/Debug Test", fileName = "New Debug Test Action")]
    public class SerializedDebugTestAction : SerializedAction<DebugTestAction, float, string, bool>
    {

    }
}
