using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.AttackTimeline
{
    public class HitboxMixerBehaviour : PlayableBehaviour
    {
        private GameObject m_boundObject;

        //bool m_shouldInitialize = true;
        bool m_boundObjectInitialStateIsActive = false;

        public static ScriptPlayable<HitboxMixerBehaviour> Create(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<HitboxMixerBehaviour>.Create(graph, inputCount);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_boundObject == null)
            {
                m_boundObject = playerData as GameObject;
                m_boundObjectInitialStateIsActive = m_boundObject != null && m_boundObject.activeSelf;
            }

            if (m_boundObject == null)
                return;

            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;

                    var input = playable.GetInput(i);

                    //InitializeData(input);

                    break;
                }
            }

            m_boundObject.SetActive(hasInput);

             //playable as PlayableAsset
        }

        //void InitializeData(Playable input)
        //{
        //    if (m_shouldInitialize && Application.isPlaying)
        //    {
        //        HitboxBehaviour hitboxBehaviour = GetHitboxBehaviour(input);
        //        HitboxData hitboxData = hitboxBehaviour.HitboxData;
        //
        //        HitboxCollider collider = m_boundObject.GetComponent<HitboxCollider>();
        //        collider.Data = hitboxData;
        //
        //        m_shouldInitialize = false;
        //    }
        //}

        //static HitboxBehaviour GetHitboxBehaviour(Playable playable)
        //{
        //    ScriptPlayable<HitboxBehaviour> hitboxInput = (ScriptPlayable<HitboxBehaviour>)playable;
        //    return hitboxInput.GetBehaviour();
        //}

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_boundObject == null)
                return;

            m_boundObject.SetActive(m_boundObjectInitialStateIsActive);
        }
    }
}

