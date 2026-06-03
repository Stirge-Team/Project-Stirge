using UnityEngine;

namespace Stirge.UtilityAI
{
    public class UtilityEnemy : MonoBehaviour
    {
        private Transform m_target;
        private float m_speed;
        private float m_maxHealth;

        private void Awake()
        {
            m_target = GameObject.FindWithTag("Player").transform;
        }
    }
}
