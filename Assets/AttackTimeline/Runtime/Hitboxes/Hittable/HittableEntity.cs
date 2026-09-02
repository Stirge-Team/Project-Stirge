using Stirge.AttackTimeline;
using UnityEngine;

namespace Stirge.Combat
{
    public class HittableEntity : Hittable
    {
        [SerializeField] private CombatEntity m_owner;

        public override void OnHit(HitboxData hitboxData, object parsedOwner = null)
        {
            CombatEntity otherEntity = (CombatEntity)parsedOwner;

            hitboxData.OnHitEffect.OnHit(m_owner, otherEntity);

            //base.OnHit();
        }
    }
}
