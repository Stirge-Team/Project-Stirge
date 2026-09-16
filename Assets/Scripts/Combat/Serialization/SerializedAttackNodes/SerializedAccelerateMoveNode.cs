using Stirge.Serialization;
using Stirge.Tools;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Local Offset", 0), NameOverride("Stopping Distance", 1), NameOverride("Consider Y Position", 2), NameOverride("Acceleration", 3), NameOverride("Max Speed", 4)]
    public class SerializedAccelerateMoveNode : SerializedAttackNode<AccelerateMoveNode, RandomVector3Field, RandomFloatField, bool, RandomFloatField, RandomFloatField> { }
}
