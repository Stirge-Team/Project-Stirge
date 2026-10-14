using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    using AttackTimeline;
    using Destroyable;
    using UtilityAI;

    public class HittableProp : Hittable
    {
        [SerializeField] private int m_health = 3;

        /// <inheritdoc/>
        public override void OnHit(CombatEntity attacker, SerializedHitboxData hitboxData, Vector3 hitPosition, Vector3 hitVelocity)
        {
            m_health -= hitboxData.Damage;

            if (m_health <= 0)
            {
                //my main goal is to
                BlowUp(hitPosition, hitVelocity.normalized);
            }
        }
        public override void OnHit(CombatEntity attacker, SerializedHitboxData hitboxData, Vector3 hitPosition, Vector3 hitVelocity, List<Status> statusesToApply = null)
        {
            OnHit(attacker, hitboxData, hitPosition, hitVelocity);
        }

        public void BlowUp(Vector3 statusPosition, Vector3 hitboxVelocityVector)
        {
            GetComponent<Destroyable>().Destroy(statusPosition, hitboxVelocityVector);
        }
    }
}
