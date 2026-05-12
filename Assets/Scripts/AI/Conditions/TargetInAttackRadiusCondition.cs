using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class TargetInAttackRadiusCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            if (agent.TargetTransform != null)
                return Vector3.Distance(agent.Transform.position, agent.TargetTransform.position) <= agent.AttackRadius;
            else
                return false;
        }
    }
}
