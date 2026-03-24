using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

        public void ReportDeath(Enemy enemy)
        {
            m_spawnedEnemies.Remove(enemy);
            SpawnEnemy();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_spawnLocation.position, 1f);
        }
        public void DebugStun(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (Enemy enemy in m_spawnedEnemies)
                {
                    enemy.EnterStun(3f);
                }
            }
        }

        public void DebugKnockback(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (Enemy enemy in m_spawnedEnemies)
                {
                    enemy.EnterKnockback(500f, new Vector2(1, 1), 3f);
                }
            }
        }
        public void DebugReduceHealth(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (Enemy enemy in m_spawnedEnemies)
                {
                    enemy.TakeDamage(1);
                }
            }
        }
        public void DebugAirJuggle(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (Enemy enemy in m_spawnedEnemies)
                {
                    enemy.EnterAirJuggle(300f, Vector3.up, 1.3f, 4f);
                }
            }
        }

        
#endif
    }
}
