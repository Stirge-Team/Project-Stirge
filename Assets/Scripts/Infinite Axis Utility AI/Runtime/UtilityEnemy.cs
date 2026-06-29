using UnityEngine;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    using Core;
    using Serialization;
    using Zor.SimpleBlackboard.Components;

    public class UtilityEnemy : MonoBehaviour
    {
        private Transform m_target;
        private float m_speed;
        private float m_maxHealth;

        [Header("Components")]
        [SerializeField] private SimpleBlackboardContainer m_blackboardContainer;
        [SerializeField] private Rigidbody m_rb;

        [Header("Utility AI")]
        [SerializeField] private SerializedActor m_actorData;
        [SerializeField] private Actor m_actor;
        [SerializeField] private Blackboard m_blackboard;

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
        public Blackboard InitialiseBlackboard()
        {
            m_blackboard = null;
            m_blackboardContainer.RecreateBlackboard();
            m_blackboard = m_blackboardContainer.blackboard;
            return m_blackboard;
        }
        #endregion
    }
}
