using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Stirge.Input
{
    using UnityEngine.Timeline;

    [System.Serializable]
    public class AttackBinding
    {
        public AttackBinding(AttackInput input, TimelineAsset timeline)
        {
            m_attackInput = input;
            m_attackTimeline = timeline;
        }
        public AttackBinding(AttackBinding binding)
        {
            m_attackInput = binding.m_attackInput;
            m_attackTimeline = binding.m_attackTimeline;
        }
        
        [SerializeField] private AttackInput m_attackInput;
        [SerializeField] private TimelineAsset m_attackTimeline;

        public AttackInput attackInput => m_attackInput;
        public TimelineAsset attackTimeline => m_attackTimeline;

        public KeyValuePair<AttackInput, TimelineAsset> ConvertToDictionaryEntry()
        {
            return new KeyValuePair<AttackInput, TimelineAsset>(m_attackInput, m_attackTimeline);
        }

        public static Dictionary<AttackInput, TimelineAsset> ConvertToDictionary(AttackBinding binding)
        {
            return new Dictionary<AttackInput, TimelineAsset>
            {
                { binding.m_attackInput, binding.m_attackTimeline }
            };
        }
        public static Dictionary<AttackInput, TimelineAsset> ConvertToDictionary(IEnumerable<AttackBinding> bindings)
        {
            return new Dictionary<AttackInput, TimelineAsset>(bindings.Select(binding => binding.ConvertToDictionaryEntry()));
        }
    }
}