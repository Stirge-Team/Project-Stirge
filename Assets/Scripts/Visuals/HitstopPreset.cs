using UnityEngine;

namespace Stirge.Visuals
{
    [CreateAssetMenu(fileName = "NewHitstopPreset", menuName = "Stirge/Hitstop Preset")]
    public class HitstopPreset : ScriptableObject
    {
        [SerializeField, Min(0), Tooltip("How long this hitstop effect should last")]
        private float m_duration = 0;
        public float Duration => m_duration;
        [SerializeField, Range(0, 1),Tooltip("What the time scale should be set to during this hitstop effect")]
        private float m_scale = 1;
        public float Scale => m_scale;

        public HitstopPreset(float dur, float scl)
        {
            m_duration = dur;
            m_scale = scl;
        }

        public bool TriggerHitstop()
        {
            TimeManager.Instance.SetTimeScaleForTime(m_scale, m_duration);
            return true;
        }
    }
}
