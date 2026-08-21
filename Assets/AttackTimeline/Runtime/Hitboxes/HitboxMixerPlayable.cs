using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.AttackTimeline
{
    public class HitboxMixerPlayable : PlayableBehaviour
    {
        private GameObject m_boundObject;

        private float m_damage;

        public static ScriptPlayable<HitboxMixerPlayable> Create(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<HitboxMixerPlayable>.Create(graph, inputCount);
        }

        public HitboxMixerPlayable() { }
        public HitboxMixerPlayable(HitboxData data)
        {
            m_damage = data.Damage;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_boundObject == null)
            {
                m_boundObject = playerData as GameObject;
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

                    //HitboxPlayableAsset asset = playable.GetInput(i).GetHandle();

                    bool isTrue = input.IsValid();

                    break;
                }
            }

            m_boundObject.SetActive(hasInput);

             //playable as PlayableAsset
        }
    }
}

