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
            return m_maxScore - Mathf.Min(m_maxScore, Vector3.SqrMagnitude(user.TargetPosition - user.Motor.GetPosition()));
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
