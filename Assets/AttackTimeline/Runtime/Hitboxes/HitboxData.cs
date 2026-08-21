using System;
using UnityEngine;

namespace Stirge.AttackTimeline
{
    using Combat;

    [Serializable]
    public class HitboxData
    {
        [SerializeField] private float m_damage;
        [SerializeField] private LayerMask m_mask;
        [SerializeField] private OnHitEffect m_onHitEffect;

        public LayerMask Mask => m_mask;
        public OnHitEffect OnHitEffect => m_onHitEffect;
        public float Damage => m_damage;
    }


}


