using UnityEngine;

namespace Stirge.AI
{
    public class IdleBehaviour : Behaviour
    {
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            Debug.Log("gaming");
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
