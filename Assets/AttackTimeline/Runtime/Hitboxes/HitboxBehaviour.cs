using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.AttackTimeline
{

    [Serializable]
    public class HitboxBehaviour : PlayableBehaviour
    {
        public HitboxData HitboxData;

        private GameObject m_boundObject;
        private bool m_initiated = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_initiated) { return; }

            if (m_boundObject == null)
            {
                m_boundObject = playerData as GameObject;
                m_initiated = true;
            }

            if (m_boundObject == null) return;


            HitboxBehaviour hitboxBehaviour = GetHitboxBehaviour(playable);
            HitboxData hitboxData = hitboxBehaviour.HitboxData;
            
            HitboxCollider collider = m_boundObject.GetComponent<HitboxCollider>();
            collider.CreateHitbox(hitboxData);

        }
        static HitboxBehaviour GetHitboxBehaviour(Playable playable)
        {
            ScriptPlayable<HitboxBehaviour> hitboxInput = (ScriptPlayable<HitboxBehaviour>)playable;
            return hitboxInput.GetBehaviour();
        }
        
    }
}


