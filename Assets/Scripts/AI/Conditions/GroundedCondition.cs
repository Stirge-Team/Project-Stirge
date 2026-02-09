using UnityEngine;

namespace Stirge.AI
{
    public class GroundedCondition : Condition
    {
        public override bool IsTrue(Agent agent)
        {
            return agent.IsGrounded;
        }
    }
}
