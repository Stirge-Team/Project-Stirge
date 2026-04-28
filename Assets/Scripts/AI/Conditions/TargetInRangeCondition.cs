using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class TargetInRangeCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            if (agent.TargetObject != null)
                return Vector3.Distance(agent.transform.position, agent.TargetObject.position) <= agent.StoppingDistance;
            else
                return false;
        }
    }
}
