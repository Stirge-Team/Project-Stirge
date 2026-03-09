using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stirge.Input
{
    [System.Flags]
    public enum AttackInput
    {
        A = 1,
        B = 2,
        X = 4,
        Y = 8
    }

    public class PlayerInputProcessing : MonoBehaviour
    {
        private Dictionary<AttackInput, Attack> m_bindings;

        public void SetBindings(Dictionary<AttackInput, Attack> bindings)
        {
            m_bindings = bindings;
        }
    }
}
