using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Actions
{
    using GenericBlackboard;
    using Core;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Transform Property", 0), NameOverride("Resource Spawner Property", 1), NameOverride("Target Property Name", 2)]
    public class SerializedSetFoodTargetAction : SerializedAction<SetFoodTargetAction, BlackboardPropertyName, BlackboardPropertyName, BlackboardPropertyName> { }
}
