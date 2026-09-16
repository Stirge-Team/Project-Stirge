using Stirge.Serialization;
using Stirge.Tools;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Stopping Distance", 0), NameOverride("Use Initial Position", 1), NameOverride("Speed", 2)]
    public class SerializedApproachTargetNode : SerializedAttackNode<ApproachTargetNode, RandomFloatField, bool, RandomFloatField> { }
}
