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

        private AirStallState m_state;
        private float m_airStallTime = 0f;
        
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
            agent.SetPhysicsMode(PhysicsMode.Physics);
            m_airStallTime = agent.RetrieveMemory<float>("AirStall");

            // if no air stall, then skip
            if (m_airStallTime <= 0)
                m_state = AirStallState.Stopped;
            else
                m_state = AirStallState.Waiting;
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
                        agent.SetPhysicsMode(PhysicsMode.Kinematic);
                    }
                    break;
                // keep agent suspended in air until time is up
                case AirStallState.Active:
                    m_airStallTime -= Time.deltaTime;
                    if (m_airStallTime <= 0)
                    {
                        m_state = AirStallState.Stopped;
                        agent.RemoveMemory("AirStall");
                        agent.SetPhysicsMode(PhysicsMode.Physics);
                    }
                    break;
            }
        }
        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
            agent.SetPhysicsMode(PhysicsMode.NavMesh);
            agent.RemoveMemory("AirStall");
            m_airStallTime = 0f;
        }

        private bool DetermineIfShouldAirStall(Agent agent)
        {
            return agent.GetVelocity().y < 0;
        }
    }
}
