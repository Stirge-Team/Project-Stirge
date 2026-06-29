using UnityEngine;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI.Serialization.SerializedAxes
{
    using Core.Axes;
    using Stirge.Serialization;

    [NameOverride("Callback Name", 0)]
    public sealed class SerializedAbsoluteAxis : SerializedAxis<AbsoluteAxis, string> { }
}
