using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.ScoringMethods
{
    [NameOverride("Use Cosine", 0), NameOverride("A", 1), NameOverride("B", 2), NameOverride("C", 3), NameOverride("D", 4)]
    public class SerializedTrigScoringMethod : SerializedScoringMethod<TrigScoringMethod, bool, float, float, float, float>
    {

    }
}
