using UnityEngine;

namespace Stirge.UtilityAI.Demo.Actions
{
    using Blackboard;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Guy Property", 0)]
    public class SerializedDepositLogAction : SerializedAction<DepositLogAction, BlackboardPropertyName> { }
}
