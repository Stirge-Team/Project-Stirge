using UnityEngine;

namespace Stirge.Combat
{
    [CreateAssetMenu(menuName = "Movement Properties", fileName = "New Movement Properties", order = 452)]
    public class MovementProperties : ScriptableObject
    {
        [SerializeField, Min(0f)] private float m_horizontalTopSpeed;
        [SerializeField, Min(0f)] private float m_acceleration;
        [SerializeField, Min(0f)] private float m_angularSpeed;
        [SerializeField, Min(0f)] private float m_friction;

        public float HorizontalTopSpeed => m_horizontalTopSpeed;
        public float Acceleration => m_acceleration;
        public float AngularSpeed => m_angularSpeed;
        public float Friction => m_friction;
    }
}
