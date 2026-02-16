using UnityEngine;

namespace Stirge.AI
{
    public class TargetInRangeCondition : Condition
    {
        public override bool IsTrue(Agent agent)
        {
            return (Vector3.Distance(agent.transform.position, agent.TargetPosition) <= agent.DetectionRadius);
        }
    }
}
