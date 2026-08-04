using UnityEngine;

namespace Stirge.UtilityAI.Demo.Actions
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Guy Property", 0)]
    public class SerializedEatFoodAction : SerializedAction<EatFoodAction, BlackboardPropertyName> { }
}
