using UnityEngine;
using Zor.SimpleBlackboard.Core;
using Zor.SimpleBlackboard.Components;

namespace Stirge.UtilityAI
{
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
        
        private Actor m_actor;
        private Blackboard m_blackboard;

        public Blackboard Blackboard => m_blackboard;

        private void Start()
        {
            Initialise();
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

        [ContextMenu("Add Random Force")]
        public void AddForceRandom()
        {
            m_rb.AddForce(new Vector3(Random.value, Random.value, Random.value), ForceMode.VelocityChange);
        }

        [ContextMenu("Initialise")]
        private void Initialise()
        {
            m_blackboard = new();
            m_actorData.CreateActor(m_blackboard);
            Debug.Log($"AI objects initialised for {GetType().Name} '{name}'!", this);
        }
    }
}
