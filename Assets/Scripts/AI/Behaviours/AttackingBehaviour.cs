using UnityEngine;

namespace Stirge.AI
{
    using Combat.Attacks;
    using Tools;

    [System.Serializable]
    public class AttackingBehaviour : Behaviour
    {
        [SerializeField] private AttackData m_attackData;
        [SerializeField] private State m_exitState;

        private AttackNode[] m_sequence;

        private Ref<AttackNode> m_currentNodeRef;
        private int m_currentNodeIndex;

        public override void _Enter(Agent agent)
        {
            m_sequence = m_attackData.EvaluateSequence();
            m_currentNodeRef = new Ref<AttackNode>(null);
            m_currentNodeIndex = -1;
            agent.Enemy.isAttacking = true;
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            // if no node is currently being processed
            if (m_currentNodeRef.Value == null)
            {
                m_currentNodeIndex++;
                
                // if reached the end of the sequence, exit this state
                if (m_currentNodeIndex >= m_sequence.Length)
                {
                    agent.EnterState(m_exitState);
                    return;
                }

                // start processing the new AttackNode
                m_currentNodeRef = new Ref<AttackNode>(m_sequence[m_currentNodeIndex]);
                agent.Enemy.StartAttackCoroutine(m_currentNodeRef);
            }
            // A node is being processed, so wait for the coroutine
        }

        public override void _Exit(Agent agent)
        {
            // apply any outstanding root motion, in case the Behaviour was exited before completion
            agent.ApplyRootMotion();

            // stop any attack coroutines on the Enemy
            if (m_currentNodeRef.Value != null)
            {
                agent.Enemy.StopAttackCoroutine(m_currentNodeRef.Value);
            }
            
            m_sequence = null;
            agent.Enemy.isAttacking = false;
        }
    }
}
