using UnityEngine;

namespace Stirge.AI
{
    using Combat.Attacks;

    [System.Serializable]
    public class AttackingBehaviour : Behaviour
    {
        [SerializeField] private AttackData m_attackData;

        public override void _Enter(Agent agent)
        {
            agent.Enemy.UseAttack(m_attackData);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            // Nodes are being processed, so wait for the Attack to end
        }

        public override void _Exit(Agent agent)
        {
            agent.Enemy.StopAttacking();
        }
    }
}
