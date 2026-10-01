using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;
    using System;

    public class DamageBuff : Status, ISetupable<ModifierType, float>
    {
        private ModifierType m_type;
        private float m_modifier;

        public override Type StatusType => typeof(DamageBuff);

        public void Setup(ModifierType modifierType, float modifier)
        {
            m_type = modifierType;
            m_modifier = modifier;
        }

        protected override void OnApplyInternal(CombatEntity target)
        {
            target.SetDamageModifier(m_type, m_modifier);
        }

        protected override void UpdateInternal(CombatEntity target)
        {
            
        }

        protected override void OnClearInternal(CombatEntity target)
        {
            target.SetDamageModifier(m_type, -m_modifier);
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return m_modifier * m_duration;
        }
    }
}
