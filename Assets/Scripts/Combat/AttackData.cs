using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    public class AttackData : ScriptableObject
    {
        [SerializeReference] private AttackNode m_root;

        public AttackNode[] EvaluateSequence()
        {
            List<AttackNode> sequence = new();
            m_root.Evaluate(sequence);
            return sequence.ToArray();
        }

        public static AttackData Create(AttackNode[] attackNodes, int[] bindings)
        {
            AttackData data = new AttackData();
            //data.m_root = rootAttackNode;
            return data;
        }
    }
}
