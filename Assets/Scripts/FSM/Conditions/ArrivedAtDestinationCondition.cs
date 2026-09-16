using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class ArrivedAtDestinationCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.NavMeshAgent.remainingDistance <= agent.StoppingDistance;
        }
    }
}