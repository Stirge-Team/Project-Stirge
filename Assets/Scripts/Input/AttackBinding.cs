using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stirge.Input
{
    using Combat;
    
    [System.Serializable]
    public class AttackBinding
    {
        public AttackBinding(AttackBinding binding)
        {
            attackInput = binding.attackInput;
            attackName = binding.attackName;
        }
        
        public AttackInput attackInput;
        public string attackName;

        public KeyValuePair<AttackInput, string> ConvertToDictionaryEntry()
        {
            return new KeyValuePair<AttackInput, string>(attackInput, attackName);
        }

        public static Dictionary<AttackInput, string> ConvertToDictionary(IEnumerable<AttackBinding> bindings)
        {
            return new Dictionary<AttackInput, string>(bindings.Select(binding => binding.ConvertToDictionaryEntry()));
        }
    }
}