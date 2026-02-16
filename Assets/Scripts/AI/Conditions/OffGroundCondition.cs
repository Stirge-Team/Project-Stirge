using UnityEngine;

namespace Stirge.AI
{
    public class OffGroundCondition : Condition
    {
        public override bool IsTrue(Agent agent)
        {
            return agent.RetrieveMemory<bool>("OffGround");
        }
    }
}
