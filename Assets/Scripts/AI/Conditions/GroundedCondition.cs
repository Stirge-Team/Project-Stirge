using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class GroundedCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.Enemy.m_isGrounded();
        }
    }
}
