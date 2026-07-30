using UnityEngine;

namespace Stirge.UtilityAI.Demo.Actions
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Nav Mesh Agent Property", 0), NameOverride("Target Demo Resource Property", 1)]
    public class SerializedMoveToTargetAction : SerializedAction<MoveToTargetAction, BlackboardPropertyName, BlackboardPropertyName> { }
}
