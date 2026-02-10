using UnityEngine;

namespace Stirge.AI
{
    public class AirJuggleBehaviour : OffGroundBehaviour
    {
        private enum AirStallState
        {
            Waiting,
            Active,
            Stopped
        }
        
        [SerializeField] private float m_airStallSpeedThreshold;

        private AirStallState m_state;
        private float m_airStallLength;
        private float m_allStallTimer;
        
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(true);
            m_state = AirStallState.Waiting;
            m_airStallLength = agent.RetrieveMemory<float>("AirStallLength");
            m_allStallTimer = 0;
            base._Enter(agent);
        }
        public override void _Update(Agent agent)
        {
            base._Update(agent);

            switch (m_state)
            {
                // check each frame for when to start the air stall
                case AirStallState.Waiting:
                    if (DetermineIfShouldAirStall(agent))
                    {
                        m_state = AirStallState.Active;
                        agent.SetGravityAcceleration(0f);
                        agent.SetVelocity(Vector3.zero);
                    }
                    break;
                // keep agent suspended in air until time is up
                case AirStallState.Active:
                    m_allStallTimer += Time.deltaTime;
                    if (m_allStallTimer >= m_airStallLength)
                    {
                        m_state = AirStallState.Stopped;
                        agent.RemoveMemory("AirStallLength");
                        agent.ResetGravityAcceleration();
                        agent.TriggerManualTransitions();
                    }
                    break;
                case AirStallState.Stopped:
                    if (offGround) agent.TriggerManualTransitions();
                    break;
            }
        }
        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(false);
            base._Exit(agent);
        }

        private bool DetermineIfShouldAirStall(Agent agent)
        {
            Vector3 velocity = agent.GetVelocity();
            print(velocity);
            return velocity.magnitude < m_airStallSpeedThreshold;
        }
    }
}
