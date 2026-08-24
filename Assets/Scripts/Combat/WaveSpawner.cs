using UnityEngine;

namespace Stirge.Combat
{
    using System.Collections.Generic;
    using Enemy;
    public class WaveSpawner : MonoBehaviour
    {
        [System.Serializable]
        public struct WaveEnemyData
        {
            [SerializeField]
            private Enemy m_type;
            [SerializeField]
            private int m_maxCount;
            private int m_spawnedCount;
            public bool canSpawn => m_spawnedCount < m_maxCount;
            public Enemy SpawnEnemy(Vector3 pos)
            {
                Enemy newEnemy = Instantiate(m_type, pos, Quaternion.identity);
                newEnemy.name = m_type.name;
                m_spawnedCount++;
                return newEnemy;
            }
        }
        [System.Serializable]
        public struct Wave
        {
            [SerializeField] //x,y are the center point, z,w are the bounds. i see *no* issue with this
            private Vector4 m_spawnArea;
            public Vector4 SpawnArea { readonly get { return m_spawnArea; } set { } }
            [SerializeField, Tooltip("The list of enemies to spawn. They will be spawned in the order listed.")]
            private WaveEnemyData[] m_enemies;
            public Enemy AttemptSpawnEnemy(Vector3 origin)
            {
                float rndXBound = Random.Range(-m_spawnArea.z / 2, m_spawnArea.z / 2);
                float rndZBound = Random.Range(-m_spawnArea.w / 2, m_spawnArea.w / 2);
                Vector3 position = origin + new Vector3(m_spawnArea.x, 0, m_spawnArea.y) + new Vector3(rndXBound, 0, rndZBound); //origin + set offset + rnd offset within range

                //TODO: Add player avoidance spawn code so that enemies don't spawn on the player

                for (int i = 0; i < m_enemies.Length; i++)
                {
                    if (m_enemies[i].canSpawn)
                    {
                        return m_enemies[i].SpawnEnemy(position);
                    }
                }
                return null;
            }
        }
        [SerializeField]
        private Wave[] m_waves;
        private int m_waveIndex = 0;
        private Wave m_currentWave => m_waves[m_waveIndex];
        [SerializeField]
        private float m_spawnRate = 1f;
        private float m_internalRateCountdown = 0f;
        private float m_waveDelay = 2f;
        private float m_internalGapCountdown = 0f;
        private List<Enemy> m_activeEnemies = new();
        [SerializeField]
        private int m_maxSpawnCount = 5;
        private ParticleInstancer m_particles;
        [SerializeField]
        private string m_spawnParticleName;

        [Header("Preview Settings")]
        [SerializeField, Range(0, 1)]
        private float m_gizmoAlpha = 1;

        void Start()
        {
            m_activeEnemies = new();
            m_particles = GetComponent<ParticleInstancer>();   
        }
        public void StartWaves()
        {
            enabled = true;
            m_activeEnemies = new();
        }
        void Update()
        {
            if (m_internalGapCountdown <= 0 && m_internalRateCountdown <= 0 && m_activeEnemies.Count < m_maxSpawnCount)
            {
                Enemy newEnemy = m_currentWave.AttemptSpawnEnemy(transform.position);
                if (newEnemy)
                {
                    //other spawn functions
                    m_internalRateCountdown = m_spawnRate; //reset countdown
                    m_activeEnemies.Add(newEnemy); //save to list
                    newEnemy.deathCallback = RemoveEnemyFromActiveList;
                    m_particles.PlayParticle(m_spawnParticleName, newEnemy.transform);
                }
                else if(m_activeEnemies.Count == 0) //there are no enemies left to spawn and there are no more living, so move on;
                {
                    m_waveIndex++;
                    m_internalGapCountdown = m_waveDelay;
                }
            }
            else if(m_internalGapCountdown > 0) m_internalGapCountdown -= Time.deltaTime; //wave gap
            else if(m_internalRateCountdown > 0) m_internalRateCountdown -= Time.deltaTime; //spawn rate
        }
        private void RemoveEnemyFromActiveList(Enemy deadGuy)
        {
            if(m_activeEnemies.Contains(deadGuy))
                m_activeEnemies.Remove(deadGuy);
        }

        void OnDrawGizmos()
        {
            foreach (var wave in m_waves)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, m_gizmoAlpha);
                Gizmos.DrawCube(transform.position + new Vector3(wave.SpawnArea.x, 0, wave.SpawnArea.y), new(wave.SpawnArea.z, 1, wave.SpawnArea.w));
            }
        }
    }
}