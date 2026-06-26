using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public abstract class AttackNode
    {
        /// <summary>
        /// Either adds itself to the <paramref name="activeNodes"/> list or adds other AttackNodes.<br/>
        /// Because <see cref="List{T}"/> is a reference type, adding to <paramref name="activeNodes"/> actually adds it to the list,
        /// where it used after this method is finished being performed.
        /// </summary>
        /// <param name="activeNodes">The list of active nodes.</param>
        public abstract void Evaluate(List<AttackNode> activeNodes);

        public static readonly System.Type[] AttackNodeTypes =
        {
            typeof(AnimationNode),
            typeof(ApproachTargetNode),
            typeof(SelectAttackNode),
            typeof(SequenceAttackNode),
            typeof(TranslateNode),
            typeof(ChanceNode),
            typeof(DelayNode),
            typeof(SimultaneousAttackNode),
            typeof(TimedMoveNode),
            typeof(CurveMoveNode),
            typeof(SpeedMoveNode),
            typeof(AccelerateMoveNode),
        };
    }
}
