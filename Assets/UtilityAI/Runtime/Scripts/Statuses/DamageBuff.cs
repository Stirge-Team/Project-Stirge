using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;
    using System;

    public class DamageBuff : Status, ISetupable<ModifierType, float, float>
    {
        private ModifierType m_type;
        private float m_modifier;
        private float m_duration;

        private float m_elapsedTime;

        public override Type statusType => typeof(DamageBuff);

        public void Setup(ModifierType modifierType, float modifier, float duration)
        {
            m_type = modifierType;
            m_modifier = modifier;
            m_duration = duration;
        }

        public override bool OnApply(CombatEntity user, CombatEntity target)
        {
            m_elapsedTime = 0f;
            target.SetDamageModifier(m_type, m_modifier);
            return false;
        }

        public override void Update(CombatEntity target)
        {
            m_elapsedTime += Time.deltaTime;
        }

        public override void OnClear(CombatEntity target)
        {
            target.SetDamageModifier(m_type, -m_modifier);
        }

        public override bool ShouldThisClear(CombatEntity target)
        {
            if (m_elapsedTime > m_duration)
                return true;

            return false;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return m_modifier * m_duration;
        }
    }
}
