using UnityEngine;

namespace Stirge.AI
{
    public class TargetInRangeCondition : Condition
    {
        public override bool IsTrue(Agent agent)
        {
            if (Vector3.Distance(agent.transform.position, agent.TargetPosition) <= agent.DetectionRadius)
                return true;
            else
                return false;
        }
    }
}
