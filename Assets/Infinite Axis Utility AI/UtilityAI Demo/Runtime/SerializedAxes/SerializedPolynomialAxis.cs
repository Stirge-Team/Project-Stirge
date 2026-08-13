using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using Blackboard;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Highest Term Power", 0), NameOverride("Term Coefficients", 1), NameOverride("Float Property", 2)]
    public class SerializedPolynomialAxis : SerializedAxis<PolynomialAxis, int, float[], BlackboardPropertyName> { }
}
