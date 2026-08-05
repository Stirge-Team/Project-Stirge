using UnityEngine;
using System.Collections.Generic;
using Stirge.Combat.Attacks;
using System.Linq;
using UnityEngine.Timeline;

namespace Stirge.Input
{
    public class PlayerInputProcessingDebug : MonoBehaviour
    {
        [SerializeField] private List<AttackInput> m_inputs;
        [SerializeField] private List<TimelineAsset> m_timelines;

        private void Update()
        {
            m_inputs.Clear();
            m_timelines.Clear();
            var bindings = PlayerInputProcessing.Instance.ComboBindingDebugList;
            foreach (AttackBinding binding in bindings)
            {
                m_inputs.Add(binding.attackInput);
                m_timelines.Add(binding.attackTimeline);
            }
        }
    }
}
