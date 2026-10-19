using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    using Combat;
    using Serialization;

    public class TargetMovementGoal : MovementGoal, ISetupable<float>
    {
        private float m_maxScore;

        public void Setup(float maxScore)
        {
            m_maxScore = maxScore;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            if (user.HasTarget)
                return m_maxScore - Mathf.Min(m_maxScore, Vector3.Distance(target.Motor.GetPosition(), user.Motor.GetPosition()));
            else
                return 0f;
        }

        public override void Perform(UtilityEnemy user, CombatEntity target)
        {
            // if user has a target and its current destination is not within stopping distance of its Target's position
            if (user.Motor.IsGrounded && target != null && Vector3.Distance(user.NavMeshAgent.destination, target.Motor.GetPosition()) > user.NavMeshAgent.stoppingDistance)
            {
                // update nav destination
                user.EnemyMotor.SetDestination(target.Motor.GetPosition());
            }
        }

        public override void Reset()
        {
            
        }
    }
}
