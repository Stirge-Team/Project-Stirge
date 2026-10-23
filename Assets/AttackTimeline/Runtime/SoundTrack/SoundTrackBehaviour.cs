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

            SoundManager.Instance.PlaySoundClipAtPosition(m_clip, Vector3.zero);
            m_played = true;
        }
        //I dont thing that this is being called but sound manager auto clears sounds so it should be fine
        public override void OnGraphStop(Playable playable)
        {
            SoundManager.Instance.StopPlayingSoundClip(m_clip);
        }
    }
}
