using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization.SerializedActions
{
    using GenericBlackboard;
    using Core.Actions;
    using Stirge.Serialization;

    [NameOverride("World Position", 0), NameOverride("Target Position Property", 1)]
    public class SerializedMoveToWorldPositionAction : SerializedAction<MoveToWorldPositionAction, Vector3, BlackboardPropertyName> { }
}
