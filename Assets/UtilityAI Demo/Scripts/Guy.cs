using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Stirge.UtilityAI.Demo
{
    using Core;
    using Serialization;

    public class Guy : MonoBehaviour
    {
        private GuyBlackboard m_blackboard;
        private Actor m_actor;

        [SerializeField] private SerializedActor m_actorData;

        [Header("Components")]
        [SerializeField] private NavMeshAgent m_navMeshAgent;
        [SerializeField] private SphereCollider m_interactionTrigger;

        [Header("References")]
        [SerializeField] private Campfire m_campfire;
        [SerializeField] private ResourceSpawner m_resourceSpawner;

        [Header("Properties")]
        [SerializeField] private float m_baseMoveSpeed;
        [SerializeField] private float m_maxWarmth;
        [SerializeField] private float m_warmthLossRate;
        [SerializeField] private float m_maxFullness;
        [SerializeField] private float m_fullnessLossRate;
        [SerializeField] private float m_foodValue;
        [SerializeField] private int m_bagSize;
        [SerializeField] private float m_interactionRadius;
        [SerializeField] private float m_actionDuration;

        private DemoResource m_targetResource;

        private float m_currentWarmth;
        private float m_currentFullness;

        private int m_logHeldCount;
        private int m_foodHeldCount;
        private float m_actionTimer;
        private ResourceType m_currentAction;

        #region Properties
        // components/references
        public NavMeshAgent navMeshAgent => m_navMeshAgent;
        public Campfire campfire => m_campfire;
        public ResourceSpawner resourceSpawner => m_resourceSpawner;

        // standard
        public DemoResource targetResource
        {
            get => m_targetResource;
            set
            {
                m_targetResource = value;
                m_navMeshAgent.SetDestination(m_targetResource.transform.position);
            }
        }
        public float baseMoveSpeed
        {
            get { return m_baseMoveSpeed; }
            set
            {
                m_baseMoveSpeed = value;
                m_navMeshAgent.speed = m_baseMoveSpeed;
            }
        }

        public float currentWarmth
        {
            get => m_currentWarmth;
            set
            {
                float diff = value - m_currentWarmth;
                ChangeWarmth(diff);
            }
        }
        public float currentFullness
        {
            get => m_currentFullness;
            set
            {
                float diff = value - m_currentFullness;
                ChangeFullness(diff);
            }
        }

        // runtime properties
        public int resourceCount => m_logHeldCount + m_foodHeldCount;
        public bool isBagFull => resourceCount < m_bagSize;
        public bool isPerformingAction => m_actionTimer > 0;

        // Scores
        public float warmthScore => m_currentWarmth / m_maxWarmth;
        public float fullnessScore => m_currentFullness / m_maxFullness;

        // constant properties
        public int bagSize => m_bagSize;
        public float interactionRadius => m_interactionRadius;
        #endregion

        private void Start()
        {
            m_currentWarmth = m_maxWarmth;
            m_currentFullness = m_maxFullness;
            m_logHeldCount = 0;
            m_foodHeldCount = 0;
            m_actionTimer = 0;

            m_navMeshAgent.speed = m_baseMoveSpeed;
            m_interactionTrigger.radius = m_interactionRadius;
            m_interactionTrigger.isTrigger = true;

            m_blackboard = new(this);
            m_actor = m_actorData.CreateActor(m_blackboard);
        }

        private void Update()
        {
            m_currentWarmth -= m_warmthLossRate * Time.deltaTime;
            if (m_currentWarmth <= 0)
            {
                // restart
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }

            m_currentFullness -= m_fullnessLossRate * Time.deltaTime;
            if (m_currentFullness <= 0)
            {
                // restart
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }

            // process any current action
            if (isPerformingAction)
            {
                m_actionTimer -= Time.deltaTime;

                // if reached 0
                if (m_actionTimer <= 0)
                {
                    if (m_currentAction == ResourceType.Log)
                    {
                        m_campfire.DepositLog();
                        m_logHeldCount--;
                    }
                    else
                    {
                        ChangeFullness(m_foodValue);
                        m_foodHeldCount--;
                    }
                }
            }

            m_actor.Update();
        }

        private void OnValidate()
        {
            if (m_navMeshAgent != null)
            {
                m_navMeshAgent.speed = m_baseMoveSpeed;
            }
            if (m_interactionTrigger != null)
            {
                m_interactionTrigger.radius = m_interactionRadius;
                m_interactionTrigger.isTrigger = true;
            }
        }

        public void PickUpResource(DemoResource resource)
        {
            if (m_logHeldCount + m_foodHeldCount < m_bagSize)
            {
                if (resource.IsTypeOfResource(ResourceType.Log))
                {
                    m_resourceSpawner.LogRemoved(resource);
                    m_logHeldCount++;
                }
                else
                {
                    m_resourceSpawner.FoodRemoved(resource);
                    m_foodHeldCount++;
                }

                Destroy(resource.gameObject);
            }
        }

        public void AddLogToCampfire()
        {
            if (!isPerformingAction && m_logHeldCount > 0)
            {
                BeginAction(ResourceType.Log);
            }
        }
        public void BeginEatingFood()
        {
            if (!isPerformingAction && m_foodHeldCount > 0)
            {
                BeginAction(ResourceType.Food);
            }
        }
        private void BeginAction(ResourceType type)
        {
            m_actionTimer = m_actionDuration;
            m_currentAction = type;
        }

        public void ChangeWarmth(float amount)
        {
            m_currentWarmth += amount;
            if (m_currentWarmth > m_maxWarmth)
                m_currentWarmth = m_maxWarmth;
        }
        public void ChangeFullness(float amount)
        {
            m_currentFullness += amount;
            if (m_currentFullness > m_maxFullness)
                m_currentFullness = m_maxFullness;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Resource"))
            {
                PickUpResource(other.GetComponentInParent<DemoResource>());
            }
        }
    }
}
