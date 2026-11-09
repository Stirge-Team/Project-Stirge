using System;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;

    public class Hitstun : Stun, ISetupable<string>
    {
        public override Type StatusType => typeof(Hitstun);

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            // arbritrary values
            return target.IsStunned ? 0f : 5f;
        }

        public override void OnUpdate(CombatEntity target)
        {
            // only countdown while grounded
            if (target.Motor.IsGrounded)
            {
                m_countdown -= Time.deltaTime;
            }
        }
    }
}
