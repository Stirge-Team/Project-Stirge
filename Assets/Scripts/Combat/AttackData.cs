using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [CreateAssetMenu(fileName = "New Attack data", menuName = "Stirge/Attack Data", order = 1)]
    public class AttackData : ScriptableObject
    {
        [SerializeReference] private AttackNode m_root;

        public AttackNode[] EvaluateSequence()
        {
            List<AttackNode> sequence = new();
            m_root.Evaluate(sequence);
            return sequence.ToArray();
        }
    }
}
