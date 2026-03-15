using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stirge.Input
{
    [System.Serializable]
    public class AttackBinding
    {
        public AttackInput attackInput;
        public Attack attack;

        public KeyValuePair<AttackInput, Attack> ConvertToDictionaryEntry()
        {
            return new KeyValuePair<AttackInput, Attack>(attackInput, attack);
        }

        public static Dictionary<AttackInput, Attack> ConvertToDictionary(IEnumerable<AttackBinding> bindings)
        {
            return new Dictionary<AttackInput, Attack>(bindings.Select(binding => binding.ConvertToDictionaryEntry()));
        }
    }
}