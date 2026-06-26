using UnityEngine;

namespace Stirge.UtilityAI
{
    using Blackboard;
    using Enemy;
    using Stirge.UtilityAI.Core;
    using Stirge.UtilityAI.Serialization;

    public class UtilityEnemy : MonoBehaviour
    {
        private Transform m_target;
        private float m_speed;
        private float m_maxHealth;

        [Header("Components")]
        [SerializeField] private Rigidbody m_rb;

        [Header("Utility AI")]
        [SerializeField] private SerializedActor m_actorData;
        [SerializeField] private Actor m_actor;
        [SerializeField] private EnemyBlackboard m_blackboard;

        private void Start()
        {
            
        }

        public bool IsGrounded()
        {
            return true;
        }    
        public float GetMovementSpeed()
        {
            return m_rb.linearVelocity.magnitude;
        }
        public Vector3 GetPosition()
        {
            return transform.position;
        }
        public Vector3 GetEulerRotation()
        {
            return transform.eulerAngles;
        }

        #region AI Setup
        [ContextMenu("Create Actor from SerializedActor")]
        private void CreateActorFromSerializedActor()
        {
            m_actorData.CreateActor(this);
            Debug.Log($"Actor created for {GetType().Name} '{name}'!", this);
        }

        public Actor CreateActorComponent()
        {
            m_actor = gameObject.AddComponent<Actor>();
            return m_actor;
        }
        public EnemyBlackboard CreateBlackboardComponent()
        {
            m_blackboard = gameObject.AddComponent<EnemyBlackboard>();
            m_blackboard.Init(this);
            return m_blackboard;
        }
        #endregion
    }
}
