using UnityEngine;

namespace Stirge.AI
{
    public class StunnedCondition : Condition
    {
        [Tooltip("If this bool is true, this Condition will return true when the Agent is stunned.\n" +
                 "If this bool is false, this Condition will return true when the Agent is NOT stuneed.")]
        [SerializeField] private bool m_returnTrueIfStunned;
        public override bool IsTrue(Agent agent)
        {
            return agent.ContainsMemory("Stun") == m_returnTrueIfStunned;
        }
    }
}
