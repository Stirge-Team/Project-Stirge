using UnityEngine;

namespace Stirge.UtilityAI.Serialization.SerializedAxes
{
    using Stirge.Serialization;
    using Core.Axes;

    [NameOverride("Score", 0)]
    public sealed class SerializedConstantAxis : SerializedAxis<ConstantAxis, float> { }
}
