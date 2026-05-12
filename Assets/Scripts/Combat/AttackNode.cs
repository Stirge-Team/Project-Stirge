using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public abstract class AttackNode
    {
        public abstract void Evaluate(List<AttackNode> activeNodes);

        public abstract float EvaluateTime();

        public static readonly System.Type[] AttackNodeTypes =
        {
            typeof(AnimationNode),
            typeof(ApproachTargetNode),
            typeof(SelectAttackNode),
            typeof(SequenceAttackNode),
            typeof(TranslateNode),
            typeof(ChanceNode),
            typeof(DelayNode),
        };
    }
}
