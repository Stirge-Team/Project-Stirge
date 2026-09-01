using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization.SerializedAxes
{
    using GenericBlackboard;
    using Core.Axes;
    using Stirge.Serialization;

    [NameOverride("Value Property", 0), NameOverride("Lower Bound", 1), NameOverride("Upper Bound", 2), NameOverride("Invert Value", 3)]
    public class SerializedClampedAbsoluteAxis : SerializedAxis<ClampedAbsoluteAxis, BlackboardPropertyName, float, float, bool> { }
}
