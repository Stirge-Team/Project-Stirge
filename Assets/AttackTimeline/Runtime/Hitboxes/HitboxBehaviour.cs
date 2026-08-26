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

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            //Debug.Log("Clip Start!");
            //
            //if (m_boundObject == null || !Application.isPlaying)
            //    return;
            //
            //HitboxCollider collider = m_boundObject.GetComponent<HitboxCollider>();
            //collider.Data = HitboxData;
            //
            ////base.OnBehaviourPlay(playable, info);
        }



        static HitboxBehaviour GetHitboxBehaviour(Playable playable)
        {
            ScriptPlayable<HitboxBehaviour> hitboxInput = (ScriptPlayable<HitboxBehaviour>)playable;
            return hitboxInput.GetBehaviour();
        }
        
        //public override void OnBehaviourPause(Playable playable, FrameData info)
        //{
        //    // Only execute in Play mode
        //    if (Application.isPlaying)
        //    {
        //        var duration = playable.GetDuration();
        //        var time = playable.GetTime();
        //        var count = time + info.deltaTime;
        //
        //        if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately((float)time, (float)duration))
        //        {
        //            // Execute your finishing logic here:
        //            Debug.Log("Clip done!");
        //        }
        //        return;
        //    }
        //}
    }
}


