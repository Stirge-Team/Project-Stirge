using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Stirge.AI
{
    using Enemy;
    
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

        [Header("Components")]
        [SerializeField] private Enemy m_enemy;
        [SerializeField] private Transform m_transform;
        [SerializeField] private Transform m_modelTransform;
        [SerializeField] private NavMeshAgent m_nav;
        [SerializeField] private Rigidbody m_rb;

        public Enemy Enemy => m_enemy;
        public Transform Transform => m_transform;

        [Header("Properties")]
        [SerializeField] private State m_defaultState;
        [SerializeField, Min(0)] private float m_targetDetectionRadius;
        [SerializeField, Min(0)] private float m_attackRadius;
        [SerializeField, Min(0)] private float m_defualtGravityAcceleration;

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

        public Transform TargetTransform => m_enemy.TargetTransform;
        public Vector3? TargetPosition
        {
            get { return m_targetPosition; }
            set { m_targetPosition = value; }
        }
        public float TargetDetectionRadius => m_targetDetectionRadius;
        public float AttackRadius => m_attackRadius;
        public float StoppingDistance => m_nav.stoppingDistance;
        public PhysicsMode PhysicsMode => m_physicsMode;

        private Dictionary<string, object> m_memory = new Dictionary<string, object>();

        #region UnityEvents
        public void Awake()
        {
            m_gravity = m_defualtGravityAcceleration;
            m_physicsMode = PhysicsMode.NavMesh;
            m_fsm = new FiniteStateMachine(m_defaultState);
        }

        public void OnEnable()
        {
            m_fsm._Enter(this);
        }

        public void Update(float deltaTime)
        {
            // move to match Nav Mesh Agent's position
            if (m_physicsMode == PhysicsMode.NavMesh)
            {
                m_transform.SetPositionAndRotation(m_nav.transform.position, m_nav.transform.rotation);
            }

            m_fsm._Update(this, deltaTime);
        }

        public void FixedUpdate()
        {
            // apply gravity
            if (m_physicsMode == PhysicsMode.Physics && !m_enemy.m_isGrounded())
            {
                m_rb.AddForce(0, -m_gravity * Time.fixedDeltaTime, 0, ForceMode.VelocityChange);
            }
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
                    case PhysicsMode.NavMesh:
                        // update the Nav Mesh Agent to the new position of the Agent
                        // try to make it a point on the Nav Mesh
                        if (NavMesh.SamplePosition(m_transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                        {
                            m_nav.transform.position = hit.position + new Vector3(0, 1f);
                        }
                        else
                        {
                            m_nav.transform.position = m_transform.position;
                        }
                        m_rb.isKinematic = true;
                        break;
                    case PhysicsMode.Physics:
                        ClearPath();
                        m_rb.isKinematic = false;
                        break;
                    case PhysicsMode.Kinematic:
                        ClearPath();
                        m_rb.isKinematic = true;
                        break;
                }
            }
        }

        public void CalculatePath()
        {
            if (m_targetPosition != null)
            {
                m_nav.SetDestination((Vector3)m_targetPosition);
            }
        }
        public void ClearPath()
        {
            m_targetPosition = null;
            m_nav.ResetPath();
        }

        public float GetNavSpeed()
        {
            return m_nav.speed;
        }
        public void SetNavSpeed(float speed)
        {
            m_nav.speed = speed;
        }
        #endregion

        #region Combat
        public void ApplyKnockback(float strength, Vector3 direction)
        {
            m_rb.linearVelocity = Vector3.zero;
            m_rb.AddForce(direction.normalized * strength, ForceMode.VelocityChange);
        }

        public void ApplyKnockback(float strength, Vector3 direction, float height)
        {
            direction = new(direction.x, height, direction.z);
            ApplyKnockback(strength, direction);
        }
        #endregion

        #region Transformation
        public void ApplyRootMotion()
        {
            if (m_modelTransform != null)
            {
                SetPosition(m_modelTransform.position);
                m_modelTransform.localPosition = Vector3.zero;
            }
        }

        public void SetPosition(Vector3 newPosition)
        {
            m_transform.position = newPosition;
            m_nav.transform.position = newPosition;
        }
        public void SetRotation(Quaternion newRotation)
        {
            m_transform.rotation = newRotation;
            m_nav.transform.rotation = newRotation;
        }

        public Vector3 GetVelocity()
        {
            if (m_physicsMode == PhysicsMode.NavMesh)
                return m_nav.velocity;
            else
                return m_rb.linearVelocity;
        }

        public void RotateTowards(Vector3 pos, float maxDelta)
        {
            Vector3 curPos = m_transform.position;
            Vector3 endForward = (new Vector3(pos.x, 0, pos.z) - new Vector3(curPos.x, 0, curPos.z)).normalized;

            m_transform.forward = Vector3.RotateTowards(m_transform.forward, endForward, maxDelta, 10f);
            m_nav.transform.rotation = m_transform.rotation;
        }

        #endregion

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
            Gizmos.DrawWireSphere(m_transform.position, m_nav.stoppingDistance);
        }
#endif
    }
}
