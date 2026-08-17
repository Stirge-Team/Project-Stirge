using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Stirge.Input
{
    using Combat.Attacks;
    using Combat.Attacks.Serialization;
    
    [System.Serializable]
    public class AttackBinding
    {
        public AttackBinding(AttackInput input, SerializedAttackData data)
        {
            attackInput = input;
            m_serializedAttackData = data;
        }
        public AttackBinding(AttackBinding binding)
        {
            attackInput = binding.attackInput;
            m_serializedAttackData = binding.m_serializedAttackData;
        }
        
        public AttackInput attackInput;
        [SerializeField] private SerializedAttackData m_serializedAttackData;

        private AttackData m_deserializedAttackData;

        public AttackData attackData
        {
            get
            {
                m_deserializedAttackData ??= m_serializedAttackData.CreateAttackData();
                return m_deserializedAttackData;
            }
        }

        public KeyValuePair<AttackInput, AttackData> ConvertToDictionaryEntry()
        {
            return new KeyValuePair<AttackInput, AttackData>(attackInput, attackData);
        }

        public static Dictionary<AttackInput, AttackData> ConvertToDictionary(AttackBinding binding)
        {
            return new Dictionary<AttackInput, AttackData>
            {
                { binding.attackInput, binding.attackData }
            };
        }
        public static Dictionary<AttackInput, AttackData> ConvertToDictionary(IEnumerable<AttackBinding> bindings)
        {
            return new Dictionary<AttackInput, AttackData>(bindings.Select(binding => binding.ConvertToDictionaryEntry()));
        }
    }
}