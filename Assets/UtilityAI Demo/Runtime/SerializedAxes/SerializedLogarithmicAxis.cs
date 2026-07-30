using UnityEngine;

namespace Stirge.UtilityAI.Demo.Axes
{
    using Blackboard;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Leading Coefficent", 0), NameOverride("Logarithm Base", 1), NameOverride("Horizontal Shift", 2), NameOverride("Vertical Shift", 3),
        NameOverride("Float Property", 4)]
    public class SerializedLogarithmicAxis : SerializedAxis<LogarithmicAxis, float, float, float, float, BlackboardPropertyName> { }
}
