using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class DistanceCondition : Condition
    {
        [SerializeField] private float m_distance;

        protected override bool _IsTrue(Agent agent)
        {
            return (Vector3.Distance(agent.transform.position, agent.TargetPosition) <= m_distance);
        }
    }        
}