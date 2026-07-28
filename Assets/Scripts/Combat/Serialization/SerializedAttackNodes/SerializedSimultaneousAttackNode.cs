using Stirge.Serialization;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Significant Node Index", 0), NameOverride("Nodes", 1)]
    public class SerializedSimultaneousAttackNode : SerializedAttackNode<SimultaneousAttackNode, int, AttackNode[]> { }
}
