using UnityEngine;

namespace Stirge.UtilityAI.Demo.Axes
{
    using Blackboard;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Leading Coefficient", 0), NameOverride("Horizontal Shift", 1), NameOverride("Vertical Shift", 2), NameOverride("Float Property", 3)]
    public class SerializedReciprocalAxis : SerializedAxis<ReciprocalAxis, float, float, float, BlackboardPropertyName> { }
}
