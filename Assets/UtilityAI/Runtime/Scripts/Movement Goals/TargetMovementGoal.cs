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
            {
                return Mathf.Min(m_maxScore, Vector3.Distance(user.Motor.GetPosition(), user.TargetPosition));
            }
            else
                return 0f;
        }

        public override void Perform(UtilityEnemy user, CombatEntity target)
        {
            // if user does not have a target, then cancel
            if (!user.HasTarget)
                return;

            // move to Navigation state if grounded and not performing an action
            if (!user.IsPerformingAction && user.Motor.IsGrounded && user.Motor.MovementState != MotorMovementState.Navigation)
            {
                user.Motor.SetMovementState(MotorMovementState.Navigation);
            }

            // once moved to Navigation state
            if (user.Motor.MovementState == MotorMovementState.Navigation)
            {
                // if not at the target
                if (!ReachedTarget(user))
                {
                    user.NavMeshAgent.SetDestination(user.TargetPosition);
                }
            }
        }

        private bool TargetMoved(UtilityEnemy user)
        {
            if (!user.NavMeshAgent.hasPath)
                return true;
            return Vector3.Distance(user.TargetPosition, user.NavMeshAgent.destination) > user.NavMeshAgent.stoppingDistance;
        }

        private bool ReachedTarget(UtilityEnemy user)
        {
            return Vector3.Distance(user.Motor.GetPosition(), user.TargetPosition) <= user.NavMeshAgent.stoppingDistance;
        }

        public override void Reset()
        {
            
        }

        public override void OnDrawGizmos(UtilityEnemy user, CombatEntity target)
        {
            if (user.HasTarget)
            {
                Gizmos.color = Color.lightBlue;
                Gizmos.DrawLine(user.Motor.GetPosition(), user.TargetPosition);
            }
        }
    }
}
