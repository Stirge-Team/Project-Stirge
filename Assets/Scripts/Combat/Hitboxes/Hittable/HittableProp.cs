using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    using Destroyable;

    public class HittableProp : Hittable
    {
        [SerializeField] private int m_health = 3;

        /// <inheritdoc/>
        public override void OnHit(CombatEntity attacker, SerializedHitboxData hitboxData, Vector3 hitPosition, Vector3 hitVelocity, int[] statusesToApplyIndices)
        {
            m_health -= hitboxData.Damage;

            if (m_health <= 0)
            {
                //my main goal is to
                BlowUp(hitPosition, hitVelocity.normalized);
            }
        }

        public void BlowUp(Vector3 statusPosition, Vector3 hitboxVelocityVector)
        {
            GetComponent<Destroyable>().Destroy(statusPosition, hitboxVelocityVector);
        }
    }
}
