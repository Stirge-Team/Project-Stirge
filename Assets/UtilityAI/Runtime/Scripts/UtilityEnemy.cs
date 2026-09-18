using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using UnityEngine.AI;

    public class UtilityEnemy : CombatEntity
    {
        [Header("Enemy Properties")]
        [SerializeField] private UtilityEnemyMotor m_motor;

        public new Transform transform => m_motor.transform;
        public UtilityEnemyMotor Motor => m_motor;
        public Rigidbody Rigidbody => m_motor.Rigidbody;
        public NavMeshAgent NavMeshAgent => m_motor.NavMeshAgent;
        
        [Header("Utility Properties")]
        [SerializeField] private SerializedAction[] m_serializedActions;
        [SerializeField] private SerializedMovementGoal_Base[] m_serializedMovementGoals;
        [SerializeField] private CombatEntity m_target;

        private Action[] m_actions;
        private MovementGoal[] m_movementGoals;

        private float m_actionTimer;
        private float m_movementGoalTimer;

        public CombatEntity Target => m_target;

        private void Start()
        {
            //Time.fixedDeltaTime = 0.333f;

            int actionCount = m_serializedActions.Length;
            m_actions = new Action[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                m_actions[i] = m_serializedActions[i].CreateRuntimeAction(this);
            }

            int movementGoalCount = m_serializedMovementGoals.Length;
            m_movementGoals = new MovementGoal[movementGoalCount];
            for (int i = 0; i < movementGoalCount; i++)
            {
                m_movementGoals[i] = m_serializedMovementGoals[i].CreateRuntimeMovementGoal();
            }
        }

        private void FixedUpdate()
        {
            if (m_actionTimer <= 0f)
            {
                Action newAction;
                foreach (var action in m_actions)
                {
                    Debug.Log($"{action.displayName}: {action.Evaluate(this, m_target)}");
                }
            }
            else
            {
                m_actionTimer -= Time.fixedDeltaTime;
            }

            if (m_movementGoalTimer <= 0f)
            {
                MovementGoal newMovementGoal;
                foreach (var movementGoal in m_movementGoals)
                {
                    Debug.Log($"{movementGoal.displayName}: {movementGoal.Evaluate(this, m_target)}");
                }
            }
            else
            {
                m_movementGoalTimer -= Time.fixedDeltaTime;
            }
        }

        #region Transformation
        public override Vector3 GetPosition()
        {
            return m_motor.transform.position;
        }
        public override void SetPosition(Vector3 newPosition)
        {
            m_motor.SetPosition(newPosition);
        }
        public override Quaternion GetRotation()
        {
            return m_motor.transform.rotation;
        }
        public override void SetRotation(Quaternion newRotation)
        {
            m_motor.SetRotation(newRotation);
        }
        public override void SetRotation(Vector3 eulerRotation)
        {
            m_motor.SetRotation(Quaternion.Euler(eulerRotation));
        }
        public override Vector3 GetForward()
        {
            return m_motor.transform.forward;
        }
        #endregion

        #region Physics
        public override bool IsGrounded()
        {
            return Physics.Raycast(m_motor.transform.position, Vector3.down, m_groundedCheckDistance, m_groundedCheckMask);
        }
        public override void MovePosition(Vector3 newPosition)
        {
            m_motor.SetPosition(newPosition);
        }
        #endregion

        #region Status
        public override void InflictStatus(Status status, CombatEntity user)
        {
            // NOTE: This needs to be changed. Statuses should be able to support:
            // - Statuses with no stacking. Adding a new identical Status will add another
            // - Statuses with no stacking. Cannot add identical Status while one already is inflicted.
            // - Statuses with stacking. Adding a new identical Status will add to its current stacks, changing its Resolve effect.
            // Timothy Cain video on Status Effects: https://www.youtube.com/watch?v=SH35RmM1BFM&t=9s
            // if inflicted with Statuses of same type, get references
            
            Type statusType = status.GetType();
            int indexOfExistingStatus = GetIndexOfStatus(statusType);
            // if matching status exists
            if (indexOfExistingStatus != -1)
            {
                switch (status.stackType)
                {
                    case StatusStackType.Stackable:
                        Status existingStackableStatus = m_inflictedStatuses[indexOfExistingStatus];
                        int existingStacks = existingStackableStatus.currentStackCount;
                        int maxStacks = existingStackableStatus.maxStacks;
                        if (existingStacks < maxStacks)
                        {
                            // add new stacks
                            existingStackableStatus.currentStackCount = Mathf.Min(existingStacks + status.currentStackCount, maxStacks);
                        }
                        break;
                    case StatusStackType.Unstackable:
                        // fall through to standard Add and Apply
                        break;
                    case StatusStackType.Unique:
                        // Exit to avoid adding multiples of Unique Status
                        return;
                    default:
                        return;
                }
            }
            m_inflictedStatuses.Add(status);
            status.OnApply(user, this);
        }

        /// <summary>
        /// Returns -1 if no <see cref="Status"/> of type <paramref name="statusType"/> was found.
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>Index of first inflicted <see cref="Status"/> with matching type.</returns>
        public int GetIndexOfStatus(Type statusType)
        {
            return m_inflictedStatuses.FindIndex(status => status.GetType() == statusType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>The number of the provided <paramref name="statusType"/> the Enemy is inflicted with.</returns>
        public int GetNumberOfInflictedStatus(Type statusType)
        {
            return m_inflictedStatuses.FindAll(status => status.statusType == statusType).Count;
        }
        #endregion
    }
}
