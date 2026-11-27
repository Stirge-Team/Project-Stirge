using UnityEngine;

namespace Stirge.Sound
{
    public class SoundClipPlayer : MonoBehaviour
    {
        [SerializeField]
        private SoundClip m_clip;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(m_clip.Loop)
            {
                SoundManager.Instance.PlaySoundClipOnObject(m_clip, transform);
            }
        }

        public void PlayAttachedClip()
        {
            SoundManager.Instance.PlaySoundClipOnObject(m_clip, transform);
        }
    }
}