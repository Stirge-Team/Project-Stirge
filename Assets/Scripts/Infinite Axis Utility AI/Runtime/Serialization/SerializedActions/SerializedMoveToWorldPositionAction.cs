using UnityEngine;

namespace Stirge.UtilityAI.Serialization.SerializedActions
{
    using Core;
    using Core.Actions;
    using Stirge.Serialization;

    [NameOverride("World Position", 0), NameOverride("Target Position Property", 1)]
    public class SerializedMoveToWorldPositionAction : SerializedAction<MoveToWorldPositionAction, Vector3, BlackboardPropertyName> { }
}
