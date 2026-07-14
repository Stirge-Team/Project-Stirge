using UnityEngine;

namespace Stirge.UtilityAI.Serialization.SerializedAxes
{
    using Core;
    using Core.Axes;
    using Stirge.Serialization;

    [NameOverride("Float Delegate", 0)]
    public sealed class SerializedAbsoluteAxis : SerializedAxis<AbsoluteAxis, BlackboardPropertyName> { }
}
