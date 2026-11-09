using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Stirge.UtilityAI;

namespace Stirge.Input
{
    public class PlayerInputProcessingDebug : MonoBehaviour
    {
        [SerializeField] private PlayerInputProcessing m_pip;
        
        [SerializeField] private AttackInput[] m_inputs;
        [SerializeField] private SerializedAction[] m_actions;
        [SerializeField] private PlayerInputProcessing.BufferedInput[] m_buffer;

        private void Update()
        {
            m_pip.GetDebugData(out AttackBinding[] comboBindings, out m_buffer);

            int bindingCount = comboBindings.Length;
            m_inputs = new AttackInput[bindingCount];
            m_actions = new SerializedAction[bindingCount];
            for (int i = 0; i < bindingCount; i++)
            {
                AttackBinding binding = comboBindings[i];
                m_inputs[i] = binding.AttackInput;
                m_actions[i] = binding.AttackAction;
            }
        }
    }
}
