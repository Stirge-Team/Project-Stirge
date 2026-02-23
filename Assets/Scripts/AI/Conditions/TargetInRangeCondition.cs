using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class TargetInRangeCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return (Vector3.Distance(agent.transform.position, agent.TargetPosition) <= agent.DetectionRadius);
        }
    }
}
