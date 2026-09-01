using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization.SerializedAxes
{
    using GenericBlackboard;
    using Core.Axes;
    using Stirge.Serialization;

    [NameOverride("Transform Property", 0), NameOverride("World Position", 1), NameOverride("Lower Bounds", 2), NameOverride("Upper Bounds", 3),
        NameOverride("Invert Value", 4)]
    public class SerializedDistanceFromWorldPositionAxis : SerializedAxis<DistanceFromWorldPositionAxis, BlackboardPropertyName, Vector3, float, float, bool> { }
}
