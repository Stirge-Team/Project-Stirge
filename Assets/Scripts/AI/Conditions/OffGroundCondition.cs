using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class OffGroundCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.RetrieveMemory<bool>("OffGround");
        }
    }
}
