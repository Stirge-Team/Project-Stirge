using Stirge.UtilityAI.Core;
using Stirge.UtilityAI.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.Demo
{
    public class Guy : MonoBehaviour
    {
        private GuyBlackboard m_blackboard;
        private Actor m_actor;

        [SerializeField] private SerializedActor m_actorData;
        [SerializeField] private float m_speed;
        [SerializeField] private Vector2 m_moveDirection;

        public float speed => m_speed;
        public Vector2 moveDirection => m_moveDirection;

        private void Start()
        {
            m_actor = m_actorData.CreateActor(m_blackboard);
        }
    }
}
