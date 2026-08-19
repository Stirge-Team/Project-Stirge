using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Stirge.Combat
{
    using System;
    using Enemy;

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private bool m_spawnOnStart;
        [SerializeField] private Enemy m_enemyPrefab;
        [SerializeField, Min(0)] private int m_targetSpawnCount;
        [SerializeField] private Transform m_spawnLocation;

        private List<Enemy> m_spawnedEnemies;

        private void Start()
        {
            if(m_enemyPrefab == null)
            {
                Debug.LogException(new NullReferenceException("No enemy prefab loaded into spawner! Please put a valid enemy prefab into this spawner."), this);
                Destroy(gameObject);
            }
            m_spawnedEnemies = new();
            if(m_spawnOnStart) FillEnemySpawns();
        }
        private void FillEnemySpawns(float count = 0)
        {
            if(count < 1) count = m_targetSpawnCount;

            for (int i = 0; i < count; i++)
            {
                float angle = 2*Mathf.PI * ((i+1)/count);
                Vector3 spawnPosition = (m_spawnLocation != null ? m_spawnLocation.position : transform.position) + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * m_enemyPrefab.Agent.NavMeshAgent.radius;
                SpawnEnemy(spawnPosition);
            }
        }

        private void SpawnEnemy()
        {
            Enemy spawnedEnemy = Instantiate(m_enemyPrefab, m_spawnLocation != null ? m_spawnLocation.position : transform.position, Quaternion.identity); //spawn the enemy either at the spawn location or here
            spawnedEnemy.spawner = this;
            spawnedEnemy.name = m_enemyPrefab.name;
            m_spawnedEnemies.Add(spawnedEnemy);
        }
        private void SpawnEnemy(Vector3 spawnPosition)
        {
            Enemy spawnedEnemy = Instantiate(m_enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemy.spawner = this;
            spawnedEnemy.name = m_enemyPrefab.name;
            m_spawnedEnemies.Add(spawnedEnemy);
        }

        public void ReportDeath(Enemy enemy)
        {
            m_spawnedEnemies.Remove(enemy);
            SpawnEnemy();
        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player") //only do anything if the player enters the trigger box
            {
                FillEnemySpawns();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_spawnLocation != null ? m_spawnLocation.position : transform.position, 1f);
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
                    enemy.EnterKnockback(10f, new Vector2(1, 1), 1.3f, 0, false);
                }
            }
        }
        public void DebugAirJuggle(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (Enemy enemy in m_spawnedEnemies)
                {
                    enemy.EnterAirJuggle(6f, Vector3.up, 1.3f, 0, false);
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
#endif
    }
}
