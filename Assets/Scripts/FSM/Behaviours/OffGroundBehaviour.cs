using UnityEngine;

namespace Stirge.AI
{
    public abstract class OffGroundBehaviour : Behaviour
    {
        [Tooltip("A buffer that prevents Behaviours that cause the Agent to leave the ground from Exiting " +
            "while the player is technically still on the ground after just Entering the State.")]
        [SerializeField] protected float m_offGroundTime;
        protected float m_offGroundTimer;

        public override void _Enter(Agent agent)
        {
            agent.isOffGround = false;
            m_offGroundTimer = m_offGroundTime;
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            if (!agent.isOffGround)
            {
                m_offGroundTimer -= deltaTime;
                if (m_offGroundTimer <= 0)
                {
                    agent.isOffGround = true;
                }
            }
        }

        public override void _Exit(Agent agent)
        {
            agent.isOffGround = true;
            base._Exit(agent);
        }
    }
}
