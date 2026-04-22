using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class AttackBehaviour : Behaviour
    {
        [SerializeField] private string m_attackName;
        [SerializeField] private bool m_hasRootMotion = false;
        
        public override void _Enter(Agent agent)
        {
            agent.UseAttack(m_attackName);
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            if (m_hasRootMotion)
            {
                agent.ApplyRootMotion();
            }
            base._Exit(agent);
        }
    }
}
