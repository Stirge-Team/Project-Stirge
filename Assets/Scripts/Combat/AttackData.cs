using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [CreateAssetMenu(fileName = "New Attack data", menuName = "Stirge/Attack Data", order = 1)]
    public class AttackData : ScriptableObject
    {
        [SerializeField] private AttackNode m_root;

        public SequenceData EvaluateSequence()
        {
            List<AttackNode> sequence = new();
            m_root.Evaluate(sequence);
            return new SequenceData(sequence, m_root.EvaluateTime());
        }
    }

    public struct SequenceData
    {
        public SequenceData(IEnumerable<AttackNode> attackNodes, float time)
        {
            sequence = attackNodes.ToArray();
            totalTime = time;
        }

        public AttackNode[] sequence { get; private set; }
        public float totalTime { get; private set; }
    }
}
