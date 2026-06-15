using UnityEngine;

namespace Stirge.Camera
{
    [CreateAssetMenu(fileName = "NewCameraShakePreset", menuName = "Stirge/CameraShakePreset", order = 1)]
    public class CameraShakePreset : ScriptableObject
    {
        [HideInInspector]
        public float AmplitudeGain => m_amp;
        [SerializeField]
        private float m_amp;
        [HideInInspector]
        public float FrequencyGain => m_freq;
        [SerializeField]
        private float m_freq;
        [HideInInspector]
        public float Duration => m_dur;
        [SerializeField]
        private float m_dur;
        [HideInInspector]
        public AnimationCurve Curve => m_curve; 
        [SerializeField]
        private AnimationCurve m_curve = new(new(0,1),new(1,0));

        public CameraShakePreset(float amp, float freq, float duration)
        {
            m_amp = amp;
            m_freq = freq;
            m_dur = duration;
            m_curve = new(new(0,1),new(1,0));
        }
    }
}