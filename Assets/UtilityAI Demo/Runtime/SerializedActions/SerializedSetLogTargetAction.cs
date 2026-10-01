using UnityEngine;

namespace Stirge.UtilityAI.Demo.Actions
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Transform Property", 0), NameOverride("Resource Spawner Property", 1), NameOverride("Target Property Name", 2)]
    public class SerializedSetLogTargetAction : SerializedAction<SetLogTargetAction, BlackboardPropertyName, BlackboardPropertyName, BlackboardPropertyName> { }
}
