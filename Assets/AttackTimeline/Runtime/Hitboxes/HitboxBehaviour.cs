using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.AttackTimeline.HitboxTrack
{
    using Combat.Hitboxes;

    [Serializable]
    public class HitboxBehaviour : PlayableBehaviour
    {
        [SerializeField] private SerializedHitboxData m_hitboxData;

        private Hitbox m_boundHitbox;
        private bool m_initialised = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_initialised)
                return;

            if (m_boundHitbox == null)
            {
                m_boundHitbox = playerData as Hitbox;
                m_initialised = true;
            }

            if (m_boundHitbox == null)
                return;

            // Enable Hitbox using found Data
            m_boundHitbox.InitHitbox(m_hitboxData);
        }

        public void SetBoundHitbox(Hitbox hitbox)
        {
            m_boundHitbox = hitbox;
        }
    }
}
