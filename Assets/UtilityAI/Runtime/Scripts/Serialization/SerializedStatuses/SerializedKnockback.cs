using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Serialization;

    [NameOverride("Strength", 0), NameOverride("Direction", 1),
        NameOverride("Local to?", "Which space the direction will be local to.", 2)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Statuses/Knockback", fileName = "New Knockback")]
    public class SerializedKnockback : SerializedStatus<Knockback, float, Vector3, Knockback.LocalTo>
    {

    }
}
