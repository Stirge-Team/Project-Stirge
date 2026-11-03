using System;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;

    public class Knockback : Stun, ISetupable<float, Vector3, Knockback.LocalTo>
    {
        private float m_strength;
        private Vector3 m_direction;
        private LocalTo m_localTo;

        public override Type StatusType => typeof(Knockback);

        public void Setup(float strength, Vector3 direction, LocalTo localTo)
        {
            m_strength = strength;
            m_direction = direction;
            m_localTo = localTo;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return target.IsStunned ? 0f : 5f;
        }

        protected override void OnApplyInternal(CombatEntity inflicter, CombatEntity target)
        {
            PlayStunAnimation(target);

            // if has a StunDuration
            if (Duration > 0f)
                target.Motor.onBecomeGrounded += OnClearInternal;

            Vector3 direction = m_localTo switch
            {
                LocalTo.World => m_direction,
                LocalTo.Inflicter => inflicter.Motor.GetRotation() * m_direction,
                LocalTo.Target => target.Motor.GetRotation() * m_direction,
                _ => m_direction,
            };
            Vector3 knockback = direction * m_strength;
            target.Motor.SetVelocity(knockback);
        }

        protected override void OnClearInternal(CombatEntity target)
        {
            // don't apply Stun until target has landed
            target.StartStun(Duration);
            target.Motor.onBecomeGrounded -= OnClearInternal;
        }

        public enum LocalTo
        {
            World,
            Inflicter,
            Target
        }
    }
}
