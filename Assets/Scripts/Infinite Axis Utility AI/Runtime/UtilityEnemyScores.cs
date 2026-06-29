using UnityEngine;

namespace Stirge.UtilityAI
{
    public class UtilityEnemyScores : MonoBehaviour
    {
        [SerializeField] private UtilityEnemy m_enemy;

        public float IsGrounded()
        {
            return m_enemy.IsGrounded() ? 1f : 0f;
        }
    }
}
