using Stirge.Serialization;
using Stirge.Tools;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Animation State Name", 0), NameOverride("Animation Clip", 1), NameOverride("Speed", 2)]
    public class SerializedAnimationNode : SerializedAttackNode<AnimationNode, string, AnimationClip, RandomFloatField> { }
}
