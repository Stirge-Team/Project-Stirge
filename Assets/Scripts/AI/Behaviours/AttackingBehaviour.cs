using UnityEngine;

namespace Stirge.AI
{
    using Combat.Attacks;

    [System.Serializable]
    public class AttackingBehaviour : Behaviour
    {
        [SerializeField] private AttackData m_attackData;

        private SequenceData m_sequenceData;

        public override void _Enter(Agent agent)
        {
            m_sequenceData = m_attackData.EvaluateSequence();
            agent.SetCurrentTimedTransitionDelay(m_sequenceData.totalTime);
            agent.Enemy.isAttacking = true;
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            // apply any outstanding root motion, in case the Behaviour was exited before completion
            agent.ApplyRootMotion();
            agent.Enemy.isAttacking = false;
            base._Exit(agent);
        }
    }
}
