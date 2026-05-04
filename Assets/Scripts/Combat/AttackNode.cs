using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public abstract class AttackNode
    {
        public abstract void Evaluate(List<AttackNode> activeNodes);

        public abstract float EvaluateTime();
    }
}
