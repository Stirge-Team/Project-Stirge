using System.Linq;
using UnityEngine;

namespace Stirge.AI
{
    public class CompositeCondition : Condition
    {
        [SerializeField] private Condition[] m_conditions;

        public override bool IsTrue(Agent agent)
        {
            if (m_conditions == null || m_conditions.Length == 0)
            {
                Debug.LogError($"Compound Condition {name} has no Conditions!");
                return false;
            }

            return m_conditions.All(c => c.IsTrue(agent));
        }
    }
}
