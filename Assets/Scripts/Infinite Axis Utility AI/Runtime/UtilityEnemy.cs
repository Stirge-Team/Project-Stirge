using UnityEngine;
using UnityEngine.AI;

namespace Stirge.UtilityAI
{
    using Core;
    using Serialization;

    using Random = UnityEngine.Random;

    public enum ActorPhysicsMode
    {
        NavMesh,
        PhysicsVelocity,
        Kinematic,
        NONE
    }

    public class UtilityEnemy : MonoBehaviour
    {
        [SerializeReference, HideInInspector] private Actor m_actor;
        private EnemyBlackboard m_blackboard;

        [Header("Components")]
        [SerializeField] private SerializedActor m_actorData;
        [SerializeField] private Transform m_transform;
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private NavMeshAgent m_navMeshAgent;

        [Header("Actor Properties")]
        [SerializeField] private Transform m_target;
        [SerializeField] private Vector3 m_targetPosition;
        [SerializeField] private bool m_hasTargetPosition;
        [SerializeField] private bool m_isGrounded;
        [SerializeField] private float m_navMeshAgentSpeed;
        [SerializeField] private float m_navMeshAgentAcceleration;
        [SerializeField] private float m_navMeshAgentStoppingDistance;
        [SerializeField] private ActorPhysicsMode m_physicsMode;

        [Header("Enemy Properties")]
        [SerializeField] private int m_maxHealthPoints;
        [SerializeField] private int m_currentHealthPoints;

        [Header("Constant Properties")]
        [SerializeField] private float m_gravityStrength;
        [SerializeField] private float m_groundedCheckRadius;
        [SerializeField] private Vector3 m_groundedCheckOffset;
        [SerializeField] private LayerMask m_collisionLayerMask;

        #region Public Properties
        // Components
        public new Transform transform => m_transform;
        public new Rigidbody rigidbody => m_rigidbody;
        public NavMeshAgent navMeshAgent => m_navMeshAgent;

        // Actor properties
        public Transform target
        {
            get => m_target;
            set
            {
                m_target = value;
                UpdateTargetPositionToTarget();
            }
        }
        public Vector3 targetPosition
        {
            get => m_targetPosition;
            set
            {
                SetNewTargetPosition(value);
            }
        }
        public bool hasTargetPosition
        {
            get => m_hasTargetPosition;
            set
            {
                // On switch
                if (value != m_hasTargetPosition)
                {
                    m_hasTargetPosition = value;
                    // If switching to true
                    if (value)
                        CalculatePath();
                    // If switching to false
                    else
                        ClearPath();
                }
            }
        }
        public bool isGrounded
        {
            get => m_isGrounded;
            set => m_isGrounded = value;
        }
        public float navMeshAgentSpeed
        {
            get => m_navMeshAgentSpeed;
            set
            {
                m_navMeshAgent.speed = value;
                m_navMeshAgentSpeed = value;
            }
        }
        public float navMeshAgentAcceleration
        {
            get => m_navMeshAgentAcceleration;
            set
            {
                m_navMeshAgent.acceleration = value;
                m_navMeshAgentAcceleration = value;
            }
        }
        public float navMeshAgentStoppingDistance
        {
            get => m_navMeshAgentStoppingDistance;
            set
            {
                m_navMeshAgent.stoppingDistance = value;
                m_navMeshAgentStoppingDistance = value;
            }
        }
        public ActorPhysicsMode physicsMode
        {
            get => m_physicsMode;
            set
            {
                SwitchPhysicsMode(value);
            }
        }

        // Enemy properties
        public int maxHealthPoints
        {
            get => m_maxHealthPoints;
            set
            {
                int diff = value = m_maxHealthPoints;
                currentHealthPoints += diff;
                m_maxHealthPoints = value;
            }
        }
        public int currentHealthPoints
        {
            get => m_currentHealthPoints;
            set
            {
                int diff = value - m_currentHealthPoints;
                if (diff > 0)
                {
                    //OnDamageTaken(diff);
                }
                m_currentHealthPoints = value;
            }
        }

        // Constant properties
        public float gravityStrength => m_gravityStrength;
        public float groundedCheckRadius => m_groundedCheckRadius;
        public Vector3 groundedCheckOffset => m_groundedCheckOffset;
        public LayerMask collisionLayerMask => m_collisionLayerMask;

        // Runtime properties
        public float distanceToTarget
        {
            get
            {
                if (target == null)
                    return Mathf.Infinity;

                return Vector3.Distance(transform.position, target.position);
            }
        }
        public float distanceToTargetPosition
        {
            get
            {
                if (m_hasTargetPosition)
                    return Vector3.Distance(transform.position, targetPosition);

                return Mathf.Infinity;
            }
        }
        public float rigidbodyLinearSpeed => m_rigidbody.linearVelocity.magnitude;
        #endregion

        #region Unity Messages
        private void Awake()
        {
            InitialiseAIComponents();
        }

        private void Start()
        {
            ApplyDefaultValues();
            m_actor.Start();
        }

        private void Update()
        {
            // Update transform to NavMeshAgent position
            if (m_physicsMode == ActorPhysicsMode.NavMesh)
            {
                Transform navMeshAgentTransform = navMeshAgent.transform;
                m_transform.SetPositionAndRotation(navMeshAgentTransform.position, navMeshAgentTransform.rotation);
            }

            m_actor.Update();
        }

        private void FixedUpdate()
        {
            m_isGrounded = GetIsGrounded();
            if (m_physicsMode == ActorPhysicsMode.PhysicsVelocity)
            {
                rigidbody.AddForce(new Vector3(0, -0.1f, 0) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
        #endregion

        public bool GetIsGrounded()
        {
            return Physics.CheckSphere(m_transform.position + m_groundedCheckOffset,
                m_groundedCheckRadius, m_collisionLayerMask, QueryTriggerInteraction.Ignore);
        }

        private void SwitchPhysicsMode(ActorPhysicsMode newPhysicsMode)
        {
            // if not equal to the existing mode
            if (newPhysicsMode != m_physicsMode && newPhysicsMode != ActorPhysicsMode.NONE)
            {
                m_physicsMode = newPhysicsMode;

                // if entering physics mode
                switch (newPhysicsMode)
                {
                    case ActorPhysicsMode.NavMesh:
                        // update the Nav Mesh Agent to the new position of the Agent
                        // try to make it a point on the Nav Mesh
                        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                        {
                            m_navMeshAgent.transform.position = hit.position + new Vector3(0, 1f);
                        }
                        else
                        {
                            m_navMeshAgent.transform.position = m_transform.position;
                        }

                        // check if there is a target position to go to
                        if (hasTargetPosition)
                        {
                            CalculatePath();
                        }

                        m_rigidbody.isKinematic = true;
                        break;
                    case ActorPhysicsMode.PhysicsVelocity:
                        ClearPath();
                        m_rigidbody.isKinematic = false;
                        break;
                    case ActorPhysicsMode.Kinematic:
                        ClearPath();
                        m_rigidbody.isKinematic = true;
                        break;
                }
            }
        }

        private void UpdateTargetPositionToTarget()
        {
            SetNewTargetPosition(target.position);
        }
        private void SetNewTargetPosition(Vector3 newTargetPosition)
        {
            m_targetPosition = newTargetPosition;
            m_hasTargetPosition = true;
            SwitchPhysicsMode(ActorPhysicsMode.NavMesh);
            CalculatePath();
        }

        public void CalculatePath()
        {
            if (m_physicsMode == ActorPhysicsMode.NavMesh)
                m_navMeshAgent.SetDestination(m_targetPosition);
        }
        public void ClearPath()
        {
            m_hasTargetPosition = false;
            m_navMeshAgent.ResetPath();
        }

        [ContextMenu("Add Random Force")]
        public void AddRandomForce()
        {
            SwitchPhysicsMode(ActorPhysicsMode.PhysicsVelocity);
            m_rigidbody.AddForce(new Vector3(Random.value, Random.value, Random.value), ForceMode.VelocityChange);
        }

        public void InitialiseAIComponents()
        {
            m_blackboard = new(this);
            m_actor = m_actorData.CreateActor(m_blackboard);
        }
        public void ClearAIComponents()
        {
            m_blackboard = null;
            m_actor = null;
        }

        [ContextMenu("Apply Default Values")]
        private void ApplyDefaultValues()
        {
            m_target = null;
            m_hasTargetPosition = false;
            m_isGrounded = true;
            m_navMeshAgent.speed = m_navMeshAgentSpeed;
            m_navMeshAgent.acceleration = m_navMeshAgentAcceleration;
            m_navMeshAgent.stoppingDistance = m_navMeshAgentStoppingDistance;
            m_physicsMode = ActorPhysicsMode.NONE;
            SwitchPhysicsMode(ActorPhysicsMode.NavMesh);
            m_currentHealthPoints = m_maxHealthPoints;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(m_navMeshAgent.destination, 0.4f);
        }
    }
}
