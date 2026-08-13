using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using Blackboard;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Leading Coefficient", 0), NameOverride("Base", 1), NameOverride("Horizontal Shift", 2), NameOverride("Vertical Shift", 3),
        NameOverride("Float Property", 4)]
    public class SerializedExponentialAxis : SerializedAxis<ExponentialAxis, float, float, float, float, BlackboardPropertyName> { }
}
