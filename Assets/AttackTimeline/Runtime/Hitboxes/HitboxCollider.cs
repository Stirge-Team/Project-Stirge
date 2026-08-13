using UnityEngine;
using System.Collections.Generic;

namespace Stirge.AttackTimeline
{
    using Combat;

    public class HitboxCollider : MonoBehaviour
    {
        [SerializeField] private HitboxData m_data;

        private List<Collider> m_savedColliders = new();

        private void OnTriggerEnter(Collider other)
        {
            //if the checked object's layer is NOT in the layer mask, do nothing
            if (!(((1 << other.gameObject.layer) & m_data.Mask.value) != 0)) return;

            //prevent repeat collisions
            if (m_savedColliders.Contains(other)) return;

            // do OnHit Shtuff
            //m_data.OnHitEffect.OnHit(other.GetComponent<CombatEntity>, );


        }

        private void OnDisable()
        {
            m_savedColliders = new();
        }
    }
}

