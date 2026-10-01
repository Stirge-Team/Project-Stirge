using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class ArrivedAtTargetCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            if (agent.TargetPosition != null)
                return Vector3.Distance(agent.Transform.position, (Vector3)agent.TargetPosition) <= agent.StoppingDistance;
            else
                return false;
        }
    }
}
