using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class HasTargetCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.TargetObject != null;
        }
    }
}
