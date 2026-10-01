using Stirge.Serialization;
using Stirge.Tools;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Local Offset", 0), NameOverride("Stopping Distance", 1), NameOverride("ConsiderYPosition", 2), NameOverride("Speed", 3)]
    public class SerializedSpeedMoveNode : SerializedAttackNode<SpeedMoveNode, RandomVector3Field, RandomFloatField, bool, RandomFloatField> { }
}
