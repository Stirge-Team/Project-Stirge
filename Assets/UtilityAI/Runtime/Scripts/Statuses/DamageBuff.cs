using Stirge.Combat;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
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
            target.ModifyDamage(m_type, m_modifier);
            return false;
        }

        public override bool Update(CombatEntity target)
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime > m_duration)
                return true;

            return false;
        }

        public override void OnClear(CombatEntity target)
        {
            target.ModifyDamage(m_type, -m_modifier);
        }

        protected override float EvaluateInternal(CombatEntity user, CombatEntity target)
        {
            return m_modifier * m_duration;
        }
    }
}
