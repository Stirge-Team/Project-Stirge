using UnityEngine;

namespace Stirge.UtilityAI.Serialization.SerializedAxes
{
    using Attributes;
    using Core.Axes;

    [NameOverride("Value Delegate Name", 0)]
    public sealed class SerializedAbsoluteAxis : SerializedAxis<AbsoluteAxis, string> { }
}
