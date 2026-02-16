using UnityEngine;

namespace Stirge.AI
{
    public class AirJuggleCondition : Condition
    {        
        public override bool IsTrue(Agent agent)
        {
            return agent.ContainsMemory("AirStallLength");
        }
    }
}
