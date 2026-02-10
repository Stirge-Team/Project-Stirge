using UnityEngine;

namespace Stirge.AI
{
    public class NotStunnedCondition : Condition
    {
        public override bool IsTrue(Agent agent)
        {
            return !agent.IsStunned;
        }
    }
}
