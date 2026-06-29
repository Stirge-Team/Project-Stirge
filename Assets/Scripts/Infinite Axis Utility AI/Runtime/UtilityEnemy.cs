using UnityEngine;

namespace Stirge.UtilityAI
{
    using Blackboard;
    using Core;
    using Serialization;

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
            // Check if enemy alreay has Actor, and if so, remove it
            if (gameObject.TryGetComponent(out Actor existingActor))
            {
#if UNITY_EDITOR
                DestroyImmediate(existingActor);
#else
                Destroy(existingActor);
#endif
            }

            m_actor = gameObject.AddComponent<Actor>();
            return m_actor;
        }
        public EnemyBlackboard CreateBlackboardComponent()
        {
            // Check if enemy already has EnemyBlackboard, and if so, reset it
            if (gameObject.TryGetComponent(out EnemyBlackboard existingBlackboard))
            {
#if UNITY_EDITOR
                DestroyImmediate(existingBlackboard);
#else
                Destroy(existingBlackboard);
#endif
            }

            m_blackboard = gameObject.AddComponent<EnemyBlackboard>();
            m_blackboard.Init(this);
            return m_blackboard;
        }
        #endregion
    }
}
