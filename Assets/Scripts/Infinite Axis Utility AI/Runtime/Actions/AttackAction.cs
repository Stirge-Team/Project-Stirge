using UnityEngine;

namespace Stirge.UtilityAI.Actions
{
    using Blackboard;
    using Core;
    using Combat;
    using Combat.Attacks;
    using Combat.Attacks.Serialization;
    using Stirge.Serialization;

    public class AttackAction : Action, ISetupable<BlackboardPropertyName, SerializedAttackData>
    {
        private BlackboardPropertyName m_combatEntityPropertyName;
        private SerializedAttackData m_serializedAttackData;

        private CombatEntity m_combatEntity;
        private AttackData m_attackData;
        
        void ISetupable<BlackboardPropertyName, SerializedAttackData>.Setup(BlackboardPropertyName combatEntityPropertyName, SerializedAttackData serializedAttackData)
        {
            m_combatEntityPropertyName = combatEntityPropertyName;
            m_serializedAttackData = serializedAttackData;
        }
        
        protected override void OnInitialise()
        {
            Blackboard.TryGetClassValue(m_combatEntityPropertyName, out m_combatEntity);
            m_attackData = m_serializedAttackData.CreateAttackData();
        }

        protected override void OnBegin()
        {
            m_combatEntity.UseAttack(m_attackData);
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnEnd()
        {
            m_combatEntity.StopAttacking();
        }
    }
}
