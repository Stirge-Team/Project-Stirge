using UnityEngine;

namespace Stirge.UtilityAI.Conditions
{
    using Combat;
    using Serialization;

    public class HasStatusCondition : Condition<IScorable>, ISetupable<string, EntityTargetType>
    {
        private string m_statusName;
        private EntityTargetType m_target;

        public void Setup(string statusName, EntityTargetType target)
        {
            m_statusName = statusName;
            m_target = target;
        }

        public override bool Evaluate(CombatEntity user, CombatEntity target)
        {
            return m_target switch
            {
                EntityTargetType.User => user.GetIndexOfStatus(m_statusName) != -1,
                EntityTargetType.Target => target.GetIndexOfStatus(m_statusName) != -1,
                _ => false,
            };
        }
    }
}
