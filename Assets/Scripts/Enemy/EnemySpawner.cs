using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy m_enemyPrefab;
        [SerializeField, Min(0)] private int m_targetEnemyCount;
        [SerializeField] private Transform m_spawnLocation;

        private List<Enemy> m_spawnedEnemies;

        private void Start()
        {
            m_spawnedEnemies = new List<Enemy>();
            for (int i = 0; i < m_targetEnemyCount; i++)
                SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            Enemy spawnedEnemy = Instantiate(m_enemyPrefab, m_spawnLocation.position, m_spawnLocation.rotation);
            spawnedEnemy.spawner = this;
            m_spawnedEnemies.Add(spawnedEnemy);
        }

        public void ReportDeath()
        {
            SpawnEnemy();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_spawnLocation.position, 1f);
        }
#endif
    }
}
