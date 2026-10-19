using UnityEngine;

namespace Stirge.UtilityAI.Conditions
{
    using Combat;
    using Serialization;

    public class HasStatusCondition : Condition<IScorable>, ISetupable<string, StatusTarget>
    {
        private string m_statusName;
        private StatusTarget m_target;

        public void Setup(string statusName, StatusTarget target)
        {
            m_statusName = statusName;
            m_target = target;
        }

        public override bool Evaluate(CombatEntity user, CombatEntity target)
        {
            return m_target switch
            {
                StatusTarget.User => user.GetIndexOfStatus(m_statusName) != -1,
                StatusTarget.Target => target.GetIndexOfStatus(m_statusName) != -1,
                _ => false,
            };
        }
    }
}
