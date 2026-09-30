using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    using Serialization;
    using Stirge.Combat;

    public class TargetMovementGoal : MovementGoal, INotSetupable
    {
        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return 5f;
        }

        public override void Perform(UtilityEnemy user, CombatEntity target)
        {
            if (user.IsNotWithinStoppingDistanceOf(user.NavMeshAgent.destination))
            {
                if (user.HasTarget)
                {
                    user.NavMeshAgent.SetDestination(user.TargetPosition);
                }
                else
                {
                    user.NavMeshAgent.SetDestination(user.Motor.GetPosition());
                }
            }
        }

        public override void Reset()
        {
            
        }
    }
}
