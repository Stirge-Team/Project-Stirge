using Stirge.Serialization;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Nodes", 0)]
    public class SerializedSelectAttackNode : SerializedAttackNode<SelectAttackNode, AttackNode[]> { }
}
