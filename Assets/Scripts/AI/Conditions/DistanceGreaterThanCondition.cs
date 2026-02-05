using UnityEngine;

namespace Stirge.AI
{
    public class DistanceGreaterThanCondition : Condition
    {
        [SerializeField] private float m_distance;

        public override bool IsTrue(Agent agent)
        {
            if (Vector3.Distance(agent.transform.position, agent.TargetPosition) > m_distance)
                return true;
            else
                return false;
        }
    }        
}