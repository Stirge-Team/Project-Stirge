using System;
using UnityEngine;

namespace Stirge.AttackTimeline.SoundTrack
{
    using Sound;
    using UnityEngine.Playables;

    [Serializable]
    public class SoundTrackBehaviour : PlayableBehaviour
    {
        [SerializeField] private SoundClip m_clip;
        private bool m_played = false;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if(m_played)
                return;

            SoundManager.Instance.PlaySoundClipAtPosition(m_clip, (playerData as Transform).position);
            m_played = true;
        }
    }
}
