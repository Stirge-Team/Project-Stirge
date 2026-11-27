using System;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;

    public class BasicPoison : Status, ISetupable<int>
    {
        private int m_damagePerTick;

        public override Type StatusType => typeof(BasicPoison);

        public override StatusStackType StackType => StatusStackType.Stackable;
        public override StatusDurationType DurationType => StatusDurationType.Conditional;

        public void Setup(int damage)
        {
            m_damagePerTick = damage;
        }

        protected override void OnApplyInternal(CombatEntity inflicter, CombatEntity target)
        {
            
        }
        public override void OnUpdate(CombatEntity target)
        {
            if (CurrentStacks == 0)
                return;

            // count down tick timer
            m_countdown -= Time.deltaTime;

            // if reached zero
            if (m_countdown <= 0f)
            {
                // take poison damage
                target.TakeDamage(m_damagePerTick);

                // reduce count by 1
                ModifyStacks(-1);

                // reset tick timer
                m_countdown += Duration;
            }
        }
        public override bool ShouldThisClear(CombatEntity target)
        {
            return CurrentStacks <= 0;
        }

        protected override void OnClearInternal(CombatEntity target)
        {
            
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return m_damagePerTick / Duration;
        }

        public override void Setup(Action action)
        {
            
        }
    }
}
