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
            agent.WriteMemory("OffGround", false);
            m_offGroundTimer = m_offGroundTime;
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            if (!agent.RetrieveMemory<bool>("OffGround"))
            {
                m_offGroundTimer -= Time.deltaTime;
                if (m_offGroundTimer <= 0)
                {
                    agent.WriteMemory("OffGround", true);
                }
            }
        }
    }
}
