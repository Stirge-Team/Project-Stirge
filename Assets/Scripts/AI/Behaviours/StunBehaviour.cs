using UnityEngine;

namespace Stirge.AI
{
    public class StunBehaviour : Behaviour
    {
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }
        public override void _Update(Agent agent)
        {
            if (!agent.ContainsMemory("Stun") || !agent.RetrieveMemory<bool>("Stun"))
            {
                agent.TriggerManualTransitions();
            }
        }
        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
