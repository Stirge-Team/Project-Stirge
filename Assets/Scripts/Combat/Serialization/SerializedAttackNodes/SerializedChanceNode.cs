using Stirge.Serialization;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Node", 0), NameOverride("Chance", 1)]
    public class SerializedChanceNode : SerializedAttackNode<ChanceNode, AttackNode, float> { }
}
