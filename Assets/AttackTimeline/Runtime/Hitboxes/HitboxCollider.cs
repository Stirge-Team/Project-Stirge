using UnityEngine;
using System.Collections.Generic;

namespace Stirge.AttackTimeline
{
    using Combat;

    [RequireComponent(typeof(Collider))]
    public class HitboxCollider : MonoBehaviour
    {
        [SerializeField] private CombatEntity m_owner;

        private HitboxData m_data = new();

        public HitboxData Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        private List<Collider> m_savedColliders = new();

        private void OnTriggerStay(Collider other)
        {
            //if the checked object's layer is NOT in the layer mask, do nothing
            if (!(((1 << other.gameObject.layer) & m_data.Mask.value) != 0)) return;

            //prevent repeat collisions
            if (m_savedColliders.Contains(other)) return;
            //add 
            m_savedColliders.Add(other);

            // do OnHit Shtuff - 
            //m_data.OnHitEffect.OnHit(other.GetComponent<CombatEntity>(), m_owner);

            Hittable test = other.GetComponent<Hittable>();

            test.OnHit(m_data, m_owner);
        }

        public void CreateHitbox(HitboxData data)
        {
            m_savedColliders = new();

            m_data = data;
        }
    }
}

