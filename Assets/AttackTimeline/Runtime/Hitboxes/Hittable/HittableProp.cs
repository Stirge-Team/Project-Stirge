
using Stirge.AttackTimeline;
using UnityEngine;

namespace Stirge.Combat
{
    using Destroyable;

    public class HittableProp : Hittable
    {
        [SerializeField] private int m_health = 3;

        public override void OnHit(HitboxData hitboxData, CombatEntity attackingEntity)   
        {
            m_health--;

            if (m_health == 0)
            {
                //my main goal is to
                BlowUp(hitboxData.HitboxWorldPosition, hitboxData.HitboxVelocityVector);
            }
        }

        public void BlowUp(Vector3 hitboxPosition, Vector3 hitboxVelocityVector)
        {
            GetComponent<Destroyable>().Destroy(hitboxPosition, hitboxVelocityVector);
        }
    }
}


