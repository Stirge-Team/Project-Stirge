using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Serialization;

    [NameOverride("Damage Per Tick", "The damage dealt to the target whenever the Poison procs.", 0)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Statuses/Basic Poison", fileName = "New Basic Poison")]
    public class SerializedBasicPoison : SerializedStatus<BasicPoison, int>
    {

    }
}
