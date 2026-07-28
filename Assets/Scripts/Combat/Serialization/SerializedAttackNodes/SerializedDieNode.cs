using Stirge.Serialization;
using Stirge.Tools;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Delay", 0)]
    public class SerializedDieNode : SerializedAttackNode<DieNode, RandomFloatField> { }
}
