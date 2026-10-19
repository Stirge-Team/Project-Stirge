using UnityEngine;

namespace Stirge.UtilityAI.Conditions
{
    using Serialization;

    [NameOverride("Status Name", 0), NameOverride("Who to Check", 1)]
    public class SerializedHasStatusCondition : SerializedCondition<HasStatusCondition, string, StatusTarget> { }
}
