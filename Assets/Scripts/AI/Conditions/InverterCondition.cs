using UnityEngine;

namespace Stirge.AI
{
    public class InverterCondition : Condition
    {
        [SerializeField] private Condition m_condition;
        public override bool IsTrue(Agent agent)
        {
            return !m_condition.IsTrue(agent);
        }
    }
}
