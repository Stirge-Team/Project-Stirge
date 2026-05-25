using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class MoveToTargetBehaviour : Behaviour
    {
        [Tooltip("If set to 0, will use the default speed set on the Nav Mesh Agent on the prefab.")]
        [SerializeField, Min(0)] private float m_speed;
        
        public override void _Enter(Agent agent)
        {
            if (m_speed > 0)
            {
                agent.SetNavSpeed(m_speed);
            }
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            if (agent.TargetPosition != null && Vector3.Distance(agent.Transform.position, (Vector3)agent.TargetPosition) > agent.StoppingDistance)
            {
                agent.CalculatePath();
            }
        }

        public override void _Exit(Agent agent)
        {
            agent.ClearPath();

            // reset speed if it was changed
            if (m_speed > 0)
                agent.SetDefaultNavSpeed();

            base._Exit(agent);
        }
    }
}
