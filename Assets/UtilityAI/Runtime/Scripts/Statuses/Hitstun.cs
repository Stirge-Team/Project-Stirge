using System;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;

    public class Hitstun : Stun, INotSetupable
    {
        public override Type StatusType => typeof(Hitstun);

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            // arbritrary values
            return target.IsStunned ? 0f : 5f;
        }
    }
}
