using UnityEngine;
using UnityEngine.AI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stirge.AI
{
    public class Agent : MonoBehaviour
    {
        [Header("Status States")]
        [SerializeField] private State m_stunState;
        [SerializeField] private State m_knockbackState;

        [Header("Agent Components")]
        [SerializeField] private FiniteStateMachine m_fsm;
        [SerializeField] private NavMeshAgent m_nav;
        [SerializeField] private Rigidbody m_rb;

        private Transform m_target;
        public Vector3 TargetPosition => m_target != null ? m_target.position : default;

        [Header("Agent Properties")]
        [SerializeField, Min(0)] private float m_detectionRadius;
        [SerializeField, Min(0)] private float m_targetRadius;
        [SerializeField, Min(0)] private float m_groundedCheckDistance;
        [SerializeField] private LayerMask m_groundedCheckMask;

        private bool m_physicsMode = false;
        private bool m_isGrounded = false;

        public float DetectionRadius => m_detectionRadius;
        public float TargetRadius => m_targetRadius;
        public bool IsGrounded => m_isGrounded;

        [Header("Combat Properties")]
        private float m_stunTimer;
        public bool IsStunned => m_stunTimer > 0;

        #region Core
        private void Start()
        {
            m_target = GameObject.FindGameObjectWithTag("Player").transform;
            m_nav.stoppingDistance = TargetRadius;
        }

        private void OnEnable()
        {
            m_fsm._Enter(this);
        }

        private void Update()
        {
            m_isGrounded = Physics.Raycast(transform.position, Vector3.down, m_groundedCheckDistance, m_groundedCheckMask);
            
            m_fsm._Update(this);

            // update stun
            if (m_stunTimer > 0)
            {
                m_stunTimer -= Time.deltaTime;
                if (m_stunTimer < 0)
                    m_stunTimer = 0;
            }

            // move to match Nav Mesh Agent's position if not in physics mode
            if (!m_physicsMode)
            {
                transform.SetPositionAndRotation(m_nav.transform.position, m_nav.transform.rotation);
            }
        }

        private void OnDisable()
        {
            m_fsm._Exit(this);
        }
        #endregion

        #region AI Controls
        public void SetTarget(Transform target)
        {
            m_target = target;
        }
        public void SetPhysicsMode(bool value)
        {
            if (value != m_physicsMode)
            {
                m_physicsMode = value;

                // if entering physics mode
                if (value)
                {
                    // stop Nav Mesh agent from moving
                    ClearPath();

                    // enable Rigidbody movement
                    m_rb.isKinematic = false;
                }
                // if exiting physics mode
                else
                {
                    // update the Nav Mesh Agent to the new position of the Agent
                    // try to make it a point on the Nav Mesh
                    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {
                        m_nav.transform.position = hit.position + new Vector3(0, 1f);
                    }
                    else
                    {
                        m_nav.transform.position = transform.position;
                    }

                    // disable Rigidbody movement
                    m_rb.isKinematic = true;
                }
            }
        }
        #endregion

        #region Pathing
        public void CalculatePathToTarget()
        {
            if (Vector3.Distance(transform.position, TargetPosition) > m_targetRadius && Vector3.Distance(TargetPosition, m_nav.pathEndPosition) > m_targetRadius)
            {
                m_nav.SetDestination(TargetPosition);
                Debug.Log("new Path created");
            }
        }
        public void ClearPath()
        {
            m_nav.ResetPath();
        }
        #endregion

        #region Combat
        public void ApplyStun(float length)
        {
            m_stunTimer = length;
            m_fsm.EnterState(this, m_stunState);
        }
        public void ApplyKnockback(float strength, Vector2 direction, float stunLength, float height = 1f)
        {
            m_stunTimer = stunLength;
            m_fsm.EnterState(this, m_knockbackState);
            SetPhysicsMode(true);
            m_rb.AddForce(new Vector3(direction.x, height, direction.y).normalized * strength);
        }
        private IEnumerator Knockback(float distance, Vector2 direction, float time, float height)
        {
            direction = direction.normalized;
            Vector3 start = transform.position;
            Vector3 end = new Vector3(start.x + direction.x * distance, start.y, start.z + direction.y * distance);

            // credit DitzelGames on YouTube for this function https://www.youtube.com/watch?v=ddakS7BgHRI
            System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            float timer = 0;
            while (timer < time)
            {
                timer += Time.deltaTime;
                float t = timer / time;
                Vector3 mid = Vector3.Lerp(start, end, t);
                transform.position = new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
                Debug.Log($"Start: {start}. End: {end}");
                yield return null;
            }

            transform.position = end;
        }

        public void OnJump()
        {
            ApplyStun(3f);
        }

        public void OnAttack()
        {
            ApplyKnockback(300, new Vector2(1, 1), 3);
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_detectionRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, m_targetRadius);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * m_groundedCheckDistance);
        }
#endif
    }
}
