using UnityEngine;

namespace Stirge.AI
{
    public class DistanceCondition : Condition
    {
        [SerializeField] private float m_distance;

        public override bool IsTrue(Agent agent)
        {
            return (Vector3.Distance(agent.transform.position, agent.TargetPosition) <= m_distance);
        }
    }        
}