using UnityEngine;

namespace Stirge.AI
{
    public abstract class OffGroundBehaviour : Behaviour
    {
        [Tooltip("A buffer that prevents Behaviours that cause the Agent to leave the ground from Exiting " +
            "while the player is technically still on the ground after just Entering the State.")]
        [SerializeField] protected float m_offGroundTime;
        protected float m_offGroundTimer;

        protected bool offGround => m_offGroundTimer >= m_offGroundTime;

        public override void _Enter(Agent agent)
        {
            m_offGroundTimer = 0;
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            if (m_offGroundTimer < m_offGroundTime)
                m_offGroundTimer += Time.deltaTime;
        }
    }
}
