using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Stirge.Input
{
    using Stirge.UtilityAI;

    [System.Serializable]
    public class AttackBinding
    {
        public AttackBinding(AttackInput input, SerializedAction action)
        {
            m_attackInput = input;
            m_attackAction = action;
        }
        public AttackBinding(AttackBinding binding)
        {
            m_attackInput = binding.m_attackInput;
            m_attackAction = binding.m_attackAction;
        }
        
        [SerializeField] private AttackInput m_attackInput;
        [SerializeField] private SerializedAction m_attackAction;

        public AttackInput AttackInput => m_attackInput;
        public SerializedAction AttackAction => m_attackAction;
    }
}