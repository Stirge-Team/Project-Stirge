using UnityEngine;

namespace Stirge.Navigation
{
    using Combat;
    using UtilityAI;

    public class UtilityEnemyNavigationTester : MonoBehaviour
    {
        [SerializeField] private UtilityEnemy m_enemy;
        [SerializeField] private Vector3[] m_targets;

        private int m_currentTarget = 0;

        private void Start()
        {
            m_enemy.Motor.SetMovementState(MotorMovementState.Navigation);
            SetDestination();
        }

        private void Update()
        {
            if (m_enemy.Motor.MovementState == MotorMovementState.Navigation)
            {
                if (!m_enemy.EnemyMotor.NavMeshAgent.hasPath)
                {
                    SetDestination();
                }

                if (ReachedTarget())
                {
                    m_currentTarget++;
                    if (m_currentTarget == m_targets.Length)
                    {
                        m_currentTarget = 0;
                    }
                    SetDestination();
                }
            }
        }

        private void SetDestination()
        {
            m_enemy.SetTargetPosition(m_targets[m_currentTarget]);
            m_enemy.EnemyMotor.NavMeshAgent.SetDestination(m_enemy.TargetPosition);
        }

        private bool ReachedTarget()
        {
            Vector3 target = m_targets[m_currentTarget];
            return Vector3.Distance(m_enemy.Motor.GetPosition(), target) <= m_enemy.EnemyMotor.NavMeshAgent.stoppingDistance;
        }

        private void OnDrawGizmos()
        {
            foreach (Vector3 target in m_targets)
            {
                Gizmos.DrawCube(target, Vector3.one / 3f);
            }
        }
    }
}
