using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Stirge.AI
{
    using Enemy;
    using Stirge.Combat;

    public enum PhysicsMode
    {
        NavMesh,
        Physics,
        Kinematic
    }

    [System.Serializable]
    public class Agent
    {   
        private FiniteStateMachine m_fsm;

        [Header("References")]
        [SerializeField] private Enemy m_enemy;
        [SerializeField] private Rigidbody m_rb;
        [SerializeField] private Transform m_transform;

        public Enemy Enemy => m_enemy;
        public Transform Transform => m_transform;

        [Header("Properties")]
        [SerializeField] private State m_defaultState;
        [SerializeField, Min(0)] private float m_defaultMoveSpeed = 3.5f;
        [SerializeField, Min(0)] private float m_stoppingDistance = 0.5f;
        [SerializeField, Min(0)] private float m_targetDetectionRadius = 10f;
        [SerializeField, Min(0)] private float m_attackRadius = 4.5f;
        [SerializeField, Min(0)] private float m_defualtGravityAcceleration = 9f;

        private Vector3? m_targetPosition;
        private PhysicsMode m_physicsMode;
        private float m_gravity;

        /// <summary>
        /// Used to determine whether the Agent has left the ground during aerial AI Behaviours.<br />
        /// Avoids aerial Behaviours from instantly exiting after a velocity is applied.<br />
        /// See <see cref="OffGroundBehaviour"/> and its children for some more info.
        /// </summary>
        [HideInInspector] public bool isOffGround;
        [HideInInspector] public float airStallLength;

        // properties
        public Transform TargetTransform => m_enemy.TargetTransform;
        public Vector3? TargetPosition
        {
            get { return m_targetPosition; }
            set { m_targetPosition = value; }
        }
        public float TargetDetectionRadius => m_targetDetectionRadius;
        public float AttackRadius => m_attackRadius;
        public PhysicsMode PhysicsMode => m_physicsMode;

        public float StoppingDistance => m_stoppingDistance;
        public float attackRadius => m_attackRadius;

        #region UnityEvents
        public void Awake()
        {
            m_fsm = new FiniteStateMachine(m_defaultState);
        }

        public void OnEnable()
        {
            m_fsm._Enter(this);
        }

        public void Update(float deltaTime)
        {
            m_fsm._Update(this, deltaTime);
        }

        public void OnDisable()
        {
            m_fsm._Exit(this);
        }
        #endregion

        #region AI
        public void EnterState(State newState)
        {
            m_fsm.EnterState(this, newState);
        }

        public void SetPhysicsMode(PhysicsMode value)
        {
            if (value != m_physicsMode)
            {
                m_physicsMode = value;

                // if entering physics mode
                switch (m_physicsMode)
                {
                    case PhysicsMode.Physics:
                        ClearPath();
                        //m_enemy.Motor.SetMovementState(MotorMovementState.Velocity);
                        break;
                    case PhysicsMode.Kinematic:
                        ClearPath();
                        //m_enemy.Motor.SetMovementState(MotorMovementState.Navigation);
                        break;
                    default:
                        ClearPath();
                        break;
                }
            }
        }

        public void CalculatePath()
        {
            if (m_targetPosition != null)
            {
                m_enemy.Motor.SetDestination((Vector3)m_targetPosition);
            }
        }
        public void ClearPath()
        {
            m_targetPosition = null;
            m_enemy.Motor.ClearDestination();
        }
        #endregion

        #region Combat
        public void ApplyKnockback(float strength, Vector3 direction)
        {
            m_enemy.Motor.ResetVelocity();
            m_enemy.Motor.AddVelocity(Vector3.ClampMagnitude(direction * strength, strength));
        }

        public void ApplyKnockback(float strength, Vector3 direction, float height)
        {
            direction = new(direction.x, height, direction.z);
            ApplyKnockback(strength, direction);
        }
        #endregion


        /* Memory code
        #region Memory
        public void WriteMemory(string key, object data)
        {
            if (m_memory.ContainsKey(key))
            {
                m_memory[key] = data;
                //Debug.LogWarning($"Overwrote key '{key}' in Agent '{m_transform}' memory.", m_transform);
            }
            else
            {
                m_memory.Add(key, data);
            }
        }
        public T RetrieveMemory<T>(string key)
        {
            if (m_memory.TryGetValue(key, out object value))
            {
                var cast = (T)value;
                if (cast != null)
                {
                    return cast;
                }
                else
                {
                    Debug.LogWarning($"Memory with key '{key}' cannot be cast to type '{nameof(T)}'.", m_transform);
                    return default;
                }
            }
            else
            {
                Debug.LogWarning($"No Memory exists with key '{key}' on Agent '{m_transform}'.", m_transform);
                return default;
            }
        }
        public bool ContainsMemory(string key)
        {
            return m_memory.ContainsKey(key);
        }
        public bool RemoveMemory(string key)
        {
            return m_memory.Remove(key);
        }
        public void ClearMemory()
        {
            m_memory.Clear();
        }
        #endregion
        */

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (m_transform == null)
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_transform.position, m_targetDetectionRadius);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(m_transform.position, m_attackRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_transform.position, m_stoppingDistance);
        }
#endif
    }
}
