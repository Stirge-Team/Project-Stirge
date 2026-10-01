using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stirge.UtilityAI.Demo
{
    using Tools;

    public class ResourceSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private DemoResource m_logPrefab;
        [SerializeField] private DemoResource m_foodPrefab;
        
        [Header("References")]
        [SerializeField] private Transform m_floorTransform;
        
        [Header("Properties")]
        [SerializeField] private RandomFloatField m_spawnDuration;
        [SerializeField, Range(0f, 1f)] private float m_logSpawnChance;
        [SerializeField, Range(0f, 1f)] private float m_foodSpawnChance;
        [SerializeField] private int m_maxSpawnCount;

        private List<DemoResource> m_spawnedLogs;
        private List<DemoResource> m_spawnedFood;

        private float m_spawnTimer;
        private bool m_waitingToSpawn;
        private int m_currentSpawnCount;

        private void Start()
        {
            m_spawnDuration.DetermineValue();
            m_spawnTimer = m_spawnDuration.Value;
            m_currentSpawnCount = 0;
        }

        private void Update()
        {
            if (m_waitingToSpawn && m_currentSpawnCount < m_maxSpawnCount)
            {
                SpawnResource();
            }
            
            if (m_spawnTimer > 0)
            {
                m_spawnTimer -= Time.deltaTime;

                if (m_spawnTimer <= 0)
                {
                    if (m_currentSpawnCount >= m_maxSpawnCount)
                    {
                        m_waitingToSpawn = true;
                    }
                    else
                    {
                        SpawnResource();
                    }
                }
            }
        }

        private void SpawnResource()
        {
            float GetRandomPointOnFloor() => (Random.value - 0.5f) * 2 * m_floorTransform.localScale.x * 5f;

            Vector3 spawnPosition = new(GetRandomPointOnFloor(), 0, GetRandomPointOnFloor());
            if (NavMesh.SamplePosition(spawnPosition, out var hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }

            bool isLog = Random.Range(0, m_logSpawnChance + m_foodSpawnChance) < m_logSpawnChance;
            Instantiate(isLog ? m_logPrefab : m_foodPrefab, spawnPosition, Quaternion.Euler(0, Random.value * 360f, 0), transform);
            m_currentSpawnCount++;

            m_spawnDuration.DetermineValue();
            m_spawnTimer = m_spawnDuration.Value;
        }

        public void LogRemoved(DemoResource log)
        {
            m_spawnedLogs.Remove(log);
            m_currentSpawnCount--;

        }
        public void FoodRemoved(DemoResource food)
        {
            m_spawnedFood.Remove(food);
            m_currentSpawnCount--;
        }

        public DemoResource GetClosestLog(Vector3 position)
        {
            if (m_spawnedLogs.Count == 0)
            {
                return null;
            }

            DemoResource closestLog = m_spawnedLogs[0];
            for (int i = 1, count = m_spawnedLogs.Count; i < count; i++)
            {
                DemoResource log = m_spawnedLogs[i];
                if (Vector3.SqrMagnitude(log.transform.position - position) < Vector3.SqrMagnitude(closestLog.transform.position - position))
                {
                    closestLog = log;
                }
            }

            return closestLog;
        }
        public DemoResource GetClosestFood(Vector3 position)
        {
            if (m_spawnedFood.Count == 0)
            {
                return null;
            }

            DemoResource closestFood = m_spawnedFood[0];
            for (int i = 1, count = m_spawnedFood.Count; i < count; i++)
            {
                DemoResource food = m_spawnedFood[i];
                if (Vector3.SqrMagnitude(food.transform.position - position) < Vector3.SqrMagnitude(closestFood.transform.position - position))
                {
                    closestFood = food;
                }
            }

            return closestFood;
        }
    }
}