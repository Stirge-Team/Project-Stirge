using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Stirge.UtilityAI;

namespace Stirge.Input
{
    public class PlayerInputProcessingDebug : MonoBehaviour
    {
        [SerializeField] private List<AttackInput> m_inputs;
        [SerializeField] private List<SerializedAction> m_actions;

        private void Update()
        {
            m_inputs.Clear();
            m_actions.Clear();
            var bindings = PlayerInputProcessing.Instance.ComboBindingDebugList;
            foreach (AttackBinding binding in bindings)
            {
                m_inputs.Add(binding.AttackInput);
                m_actions.Add(binding.AttackAction);
            }
        }
    }
}
