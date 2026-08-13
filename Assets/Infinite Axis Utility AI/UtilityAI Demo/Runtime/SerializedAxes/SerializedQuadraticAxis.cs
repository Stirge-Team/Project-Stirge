using UnityEngine;

namespace Stirge.UtilityAI.Demo.Axes
{
    using Blackboard;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("A", 0), NameOverride("B", 1), NameOverride("C", 2), NameOverride("Float Property", 3)]
    public class SerializedQuadraticAxis : SerializedAxis<QuadraticAxis, float, float, float, BlackboardPropertyName> { }
}
