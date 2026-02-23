using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stirge.AI
{
    public enum PhysicsMode
    {
        NavMesh,
        Physics,
        Kinematic
    }

    [System.Serializable]
    public class Agent
    {   
        [Header("Agent Components")]
        [SerializeField] private Transform m_transform;
        [SerializeField] private FiniteStateMachine m_fsm;
        [SerializeField] private NavMeshAgent m_nav;
        [SerializeField] private Rigidbody m_rb;

        private Transform m_target;

        public Transform transform => m_transform;
        public Vector3 TargetPosition => m_target != null ? m_target.position : default;

        [Header("Agent Properties")]
        [SerializeField, Min(0)] private float m_detectionRadius;
        [SerializeField, Min(0)] private float m_groundedCheckDistance;
        [SerializeField] private LayerMask m_groundedCheckMask;
        [SerializeField, Min(0)] private float m_defualtGravityAcceleration;

        private PhysicsMode m_physicsMode;
        private float m_gravity;

        public float DetectionRadius => m_detectionRadius;
        public float StoppingDistance => m_nav.stoppingDistance;
        public PhysicsMode PhysicsMode => m_physicsMode;

        private Dictionary<string, object> m_memory = new Dictionary<string, object>();

        #region Core
        public void Start()
        {
            m_target = GameObject.FindGameObjectWithTag("Player").transform;
            m_gravity = m_defualtGravityAcceleration;
            m_physicsMode = PhysicsMode.NavMesh;
        }

        public void OnEnable()
        {
            m_fsm._Enter(this);
        }

        public void Update()
        {
            // move to match Nav Mesh Agent's position
            if (m_physicsMode == PhysicsMode.NavMesh)
            {
            m_transform.SetPositionAndRotation(m_nav.transform.position, m_nav.transform.rotation);
            }

            m_fsm._Update(this);
        }

        public void FixedUpdate()
        {
            bool isGrounded = Physics.Raycast(m_transform.position, Vector3.down, m_groundedCheckDistance, m_groundedCheckMask);
            WriteMemory("Grounded", isGrounded);

            // apply gravity
            if (m_physicsMode == PhysicsMode.Physics && !isGrounded)
            {
                m_rb.AddForce(0, -m_gravity * Time.deltaTime, 0, ForceMode.VelocityChange);
            }
        }

        public void OnDisable()
        {
            m_fsm._Exit(this);
        }
        #endregion

        #region Controls
        public void EnterState(State newState)
        {
            m_fsm.EnterState(this, newState);
        }

        public void SetTarget(Transform target)
        {
            m_target = target;
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

        public void ApplyKnockback(float strength, Vector3 direction)
        {
            m_rb.linearVelocity = Vector3.zero;
            m_rb.AddForce(direction.normalized * strength);
        }
        public void ApplyKnockback(float strength, Vector2 direction, float height)
        {
            m_rb.linearVelocity = Vector3.zero;
            m_rb.AddForce(new Vector3(direction.x, height, direction.y).normalized * strength);
        }

        public void CalculatePathToTarget()
        {
            if (Vector3.Distance(m_transform.position, TargetPosition) > StoppingDistance && Vector3.Distance(TargetPosition, m_nav.pathEndPosition) > StoppingDistance)
            {
                m_nav.SetDestination(TargetPosition);
            }
        }
        public void ClearPath()
        {
            m_nav.ResetPath();
        }

        public Vector3 GetVelocity()
        {
            if (m_physicsMode == PhysicsMode.NavMesh)
                return m_nav.velocity;
            else
                return m_rb.linearVelocity;
        }

        /*
        private IEnumerator Knockback(float distance, Vector2 direction, float time, float height)
        {
            direction = direction.normalized;
            Vector3 start = m_transform.position;
            Vector3 end = new Vector3(start.x + direction.x * distance, start.y, start.z + direction.y * distance);

            // credit DitzelGames on YouTube for this function https://www.youtube.com/watch?v=ddakS7BgHRI
            System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            float timer = 0;
            while (timer < time)
            {
                timer += Time.deltaTime;
                float t = timer / time;
                Vector3 mid = Vector3.Lerp(start, end, t);
                m_transform.position = new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
                Debug.Log($"Start: {start}. End: {end}");
                yield return null;
            }

            m_transform.position = end;
        }
        */
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
                    Debug.LogError($"Memory with key '{key}' cannot be cast to type '{nameof(T)}'.", m_transform);
                    return default;
                }
            }
            else
            {
                Debug.LogError($"No Memory exists with key '{key}' on Agent '{m_transform}'.", m_transform);
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
            Gizmos.DrawWireSphere(m_transform.position, m_detectionRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_transform.position, m_nav.stoppingDistance);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(m_transform.position, m_transform.position + Vector3.down * m_groundedCheckDistance);
        }
#endif
    }
}
