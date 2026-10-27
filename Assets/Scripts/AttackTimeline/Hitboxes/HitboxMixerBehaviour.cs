using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.AttackTimeline.HitboxTrack
{
    using Combat.Hitboxes;

    public class HitboxMixerBehaviour : PlayableBehaviour
    {
        private Hitbox m_boundHitbox;

        //bool m_shouldInitialize = true;
        bool m_boundObjectInitialStateIsActive = false;

        public static ScriptPlayable<HitboxMixerBehaviour> Create(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<HitboxMixerBehaviour>.Create(graph, inputCount);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_boundHitbox == null)
            {
                m_boundHitbox = playerData as Hitbox;
                m_boundObjectInitialStateIsActive = m_boundHitbox != null && m_boundHitbox.isActive;
            }

            if (m_boundHitbox == null)
                return;

            // hasInput means that a clip is currently playing on this track
            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;
                    break;
                }
            }

            // change the active state
            m_boundHitbox.isActive = hasInput;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_boundHitbox == null)
                return;

            m_boundHitbox.isActive = m_boundObjectInitialStateIsActive;
        }
    }
}

