using System;
using UnityEngine;

namespace Stirge.AttackTimeline
{
    using Combat;

    [Serializable]
    public class HitboxData
    {
        [SerializeField] private OnHitEffect m_onHitEffect;
        [SerializeField] private LayerMask m_mask;

        public OnHitEffect OnHitEffect => m_onHitEffect;
        public LayerMask Mask => m_mask;
    }


}


