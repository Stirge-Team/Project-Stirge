
using Stirge.AttackTimeline;
using UnityEngine;

namespace Stirge.Combat
{
    public class HittableProp : Hittable
    {
        [SerializeField] private int m_health = 3;


        public override void OnHit(HitboxData hitboxData, object parsedOwner = null)
        {
            m_health--;

            if (m_health == 0)
            {
                BlowUp();
            }
        }

        public void BlowUp()
        {
            Destroy(gameObject);
        }
    }
}


