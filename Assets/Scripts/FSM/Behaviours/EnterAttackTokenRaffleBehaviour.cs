using Stirge.Combat;
using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class EnterAttackTokenRaffleBehaviour : Behaviour
    {
        public override void _Enter(Agent agent)
        {
            //entering the attack token raffle
            if (agent.TargetTransform != null) //if there is a target
            {
                if (AttackTokenDispenser.instance != null)
                    AttackTokenDispenser.instance.EnterAttackRaffle(agent.Enemy, new ScoringMethods.DistanceScore(agent.Transform, agent.TargetTransform)); //enter the raffle
                else
                    agent.Enemy.GiveToken();
            }
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }
    }
}
