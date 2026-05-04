using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class AirJuggleBehaviour : OffGroundBehaviour
    {
        private enum AirStallState
        {
            Waiting,
            Active,
            Stopped
        }

        private AirStallState m_state;
        private float m_airStallTime = 0f;
        
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
            agent.SetPhysicsMode(PhysicsMode.Physics);
            m_airStallTime = agent.airStallLength;

            // if no air stall, then skip
            if (m_airStallTime <= 0)
                m_state = AirStallState.Stopped;
            else
                m_state = AirStallState.Waiting;
        }
        public override void _Update(Agent agent, float deltaTime)
        {
            base._Update(agent, deltaTime);

            switch (m_state)
            {
                // check each frame for when to start the air stall
                case AirStallState.Waiting:
                    if (DetermineIfShouldAirStall(agent))
                    {
                        m_state = AirStallState.Active;
                        agent.SetPhysicsMode(PhysicsMode.Kinematic);
                    }
                    break;
                // keep agent suspended in air until time is up
                case AirStallState.Active:
                    m_airStallTime -= deltaTime;
                    if (m_airStallTime <= 0)
                    {
                        m_state = AirStallState.Stopped;
                        agent.airStallLength = 0f;
                        agent.SetPhysicsMode(PhysicsMode.Physics);
                    }
                    break;
            }
        }
        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
            agent.SetPhysicsMode(PhysicsMode.NavMesh);
            agent.airStallLength = 0f;
            m_airStallTime = 0f;
        }

        private bool DetermineIfShouldAirStall(Agent agent)
        {
            return agent.GetVelocity().y < 0;
        }
    }
}
