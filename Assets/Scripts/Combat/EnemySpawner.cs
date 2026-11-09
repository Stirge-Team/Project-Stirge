using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat
{
    using UtilityAI;

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private bool m_spawnOnStart;
        [SerializeField] private GameObject m_enemyPrefab;
        [SerializeField, Min(0)] private int m_targetSpawnCount;
        [SerializeField] private Transform m_spawnLocation;
        [SerializeField] private Vector3 m_spawnLocationRandomness;

        private readonly List<UtilityEnemy> m_spawnedEnemies = new();

        private void Start()
        {
            if (m_enemyPrefab == null)
            {
                Debug.LogException(new System.NullReferenceException("No enemy prefab loaded into spawner! Please put a valid enemy prefab into this spawner."), this);
                enabled = false;
            }
            m_spawnedEnemies.Clear();
            if (m_spawnOnStart)
            {
                FillSpawns();
            }
        }

        private void FillSpawns()
        {
            for (int i = 0; i < m_targetSpawnCount; i++)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            Vector3 spawnedPosition = m_spawnLocation != null ? m_spawnLocation.position : transform.position;
            // each axis has random value in range -value to +value
            spawnedPosition += new Vector3((Random.value * 2f - 1f) * m_spawnLocationRandomness.x,
                                           (Random.value * 2f - 1f) * m_spawnLocationRandomness.y,
                                           (Random.value * 2f - 1f) * m_spawnLocationRandomness.z);
            UtilityEnemy spawnedEnemy = Instantiate(m_enemyPrefab, spawnedPosition, Quaternion.Euler(0, Random.value * 360f, 0)).GetComponentInChildren<UtilityEnemy>();
            spawnedEnemy.name = m_enemyPrefab.name;
            spawnedEnemy.onDeath += ReportDeath;
            m_spawnedEnemies.Add(spawnedEnemy);
        }

        public void ReportDeath(UtilityEnemy enemy)
        {
            m_spawnedEnemies.Remove(enemy);
            SpawnEnemy();
        }

        public void OnTriggerEnter(Collider other)
        {
            // if not immediately active on start, only do anything if the player enters the trigger box
            if (!m_spawnOnStart && other.gameObject.CompareTag("Player")) 
            {
                FillSpawns();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 pos = m_spawnLocation != null ? m_spawnLocation.position : transform.position;
            Gizmos.color = Color.limeGreen;
            Gizmos.DrawCube(pos, m_spawnLocationRandomness);
        }

        [ContextMenu("Knockback All Enemies")]
        public void DebugKnockback()
        {
            foreach (UtilityEnemy enemy in m_spawnedEnemies)
            {
                //enemy.EnterKnockback(10f, new Vector2(1, 1), 1.3f, 0, false);
            }
        }
        [ContextMenu("Damage All Enemies")]
        public void DebugReduceHealth()
        {
            foreach (UtilityEnemy enemy in m_spawnedEnemies)
            {
                enemy.TakeDamage(1);
            }
        }
#endif
    }
}
