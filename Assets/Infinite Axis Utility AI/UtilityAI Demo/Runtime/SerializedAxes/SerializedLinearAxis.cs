using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using GenericBlackboard;
    using Core.Axes;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Slope", 0), NameOverride("Horizontal Shift", 1), NameOverride("Vertical Shift", 2), NameOverride("Float Property", 3)]
    public class SerializedLinearAxis : SerializedAxis<LinearAxis, float, float, float, BlackboardPropertyName> { }
}
