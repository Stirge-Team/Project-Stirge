using UnityEngine;

namespace Stirge.AI
{
    public class AirJuggleCondition : Condition
    {
        [SerializeField] private bool m_returnTrueIfGettingAirJuggled = true;
        
        public override bool IsTrue(Agent agent)
        {
            return agent.ContainsMemory("AirStallLength") == m_returnTrueIfGettingAirJuggled;
        }
    }
}
