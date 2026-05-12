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

        public override void _Enter(Agent agent)
        {
            agent.Enemy.UseAttack(m_attackData);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            // Nodes are being processed, so wait for the Attack to end
            if (!agent.Enemy.IsAttacking)
                agent.EnterState(m_exitState);
        }

        public override void _Exit(Agent agent)
        {

        }
    }
}
