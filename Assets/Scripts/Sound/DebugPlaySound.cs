using UnityEngine;

namespace Stirge.Sound
{
    public class DebugPlaySound : MonoBehaviour
    {
        [SerializeField] private SoundClip m_soundClip;
        [SerializeField] private bool m_playSound;
        [SerializeField] private bool m_stopSound;

        private void Update()
        {
            if (m_playSound)
            {
                SoundManager.Instance.PlaySoundClipOnObject(m_soundClip, transform);
                m_playSound = false;
            }

            if (m_stopSound)
            {
                SoundManager.Instance.StopPlayingSoundClip(m_soundClip);
                m_stopSound = false;
            }
        }
    }
}
