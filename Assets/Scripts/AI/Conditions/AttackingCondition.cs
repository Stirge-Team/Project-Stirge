using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class AttackingCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.Enemy.isAttacking;
        }
    }
}
