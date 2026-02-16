using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class StunnedCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.ContainsMemory("Stun");
        }
    }
}
