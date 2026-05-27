using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Stirge.Input
{
    using Combat.Attacks;
    
    [System.Serializable]
    public class AttackBinding
    {
        public AttackBinding(AttackBinding binding)
        {
            attackInput = binding.attackInput;
            attackData = binding.attackData;
        }
        
        public AttackInput attackInput;
        public AttackData attackData;

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