using UnityEngine;

namespace Stirge.UtilityAI.Serialization.SerializedAxes
{
    using Core;
    using Core.Axes;
    using Stirge.Serialization;

    [NameOverride("Value Property", 0), NameOverride("Lower Bound", 1), NameOverride("Upper Bound", 2), NameOverride("Invert Value", 3)]
    public class SerializedClampedAbsoluteAxis : SerializedAxis<ClampedAbsoluteAxis, BlackboardPropertyName, float, float, bool> { }
}
