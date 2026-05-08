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

            //looking for the anim to player -- halen idk if you're working on this but i'm adding to cuz its for the week plan :P feel free to overwrite this (all **5** painstaking minutes~)
            AttackNode[] nodes = m_attackData.EvaluateSequence().sequence;
            foreach(var node in nodes)
            {
                if (node.GetType() == typeof(AnimationNode))
                {
                    agent.Enemy.UseAttack((node as AnimationNode).Animation.name); // the code of all time...
                }
            }
            
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
