using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Actions
{
    using GenericBlackboard;
    using Core;
    using Stirge.Serialization;
    using Serialization;

    [NameOverride("Guy Property", 0)]
    public class SerializedEatFoodAction : SerializedAction<EatFoodAction, BlackboardPropertyName> { }
}
