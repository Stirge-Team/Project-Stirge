using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class HasAttackTokenCondition : Condition
    {
        protected override bool _IsTrue(Agent agent)
        {
            return agent.Enemy.AttackToken;
        }
    }
}