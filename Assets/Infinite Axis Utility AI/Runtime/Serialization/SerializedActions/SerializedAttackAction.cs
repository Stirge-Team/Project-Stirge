using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization.SerializedActions
{
    using GenericBlackboard;
    using Core;
    using Actions;
    using Stirge.Serialization;
    using Serialization;
    using Stirge.Combat;
    using Stirge.Combat.Attacks.Serialization;

    [NameOverride("Combat Entity Property", 0), NameOverride("Serialized Attack Data", 1)]
    public class SerializedAttackAction : SerializedAction<AttackAction, BlackboardPropertyName, SerializedAttackData> { }
}
