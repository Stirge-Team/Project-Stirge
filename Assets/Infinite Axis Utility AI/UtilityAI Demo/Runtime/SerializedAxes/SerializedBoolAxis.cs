using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization.SerializedAxes
{
    using Blackboard;
    using Core.Axes;
    using Stirge.Serialization;

    [NameOverride("Bool Property", 0), NameOverride("Invert Value", 1)]
    public class SerializedBoolAxis : SerializedAxis<BoolAxis, BlackboardPropertyName, bool> { }
}
