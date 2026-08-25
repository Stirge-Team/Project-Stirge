using UnityEngine;

namespace Stirge.Combat
{
    using System.Collections.Generic;
    using System.Linq;
    using Enemy;
    using UnityEngine.Events;

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
        public bool _wavesCompleted { get { return m_waveIndex >= m_waves.Count(); } }
        [SerializeField]
        private UnityEvent m_completeEvent;

        [SerializeField]
        private float m_waveDelay = 2f;
        private float m_waveDelayCountdown = 0f;
        [SerializeField]
        private float m_spawnRate = 1f;
        private float m_spawnRateCountdown = 0f;
        [SerializeField, Tooltip("How many enemies (within the maximum) should be spawned at once.")]
        private int m_batchSpawn = 1;
        [SerializeField, Tooltip("Can the number of enemies exceed the maximum limit if spawning a batch of enemies would cause it to do so. i.e. enemy limit is 4, spawning in batches of 3. First batch spawns in, then the second resulting in 6 enemies rather then 4.")]
        private bool m_canBatchOverflow = false;
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
            StartWaves();
        }
        public void StartWaves()
        {
            enabled = true;
            m_particles = GetComponent<ParticleInstancer>();
            m_activeEnemies = new();
            m_waveDelayCountdown = m_waveDelay;
            m_spawnRateCountdown = m_spawnRate;
            if (m_batchSpawn > m_maxSpawnCount && m_canBatchOverflow) Debug.LogWarning("Enemy batch spawn count exceeds the maximum enemy count. Please keep the Batch Spawn value less then or equal to the max limit unless this was intened.");
        }
        public void StopWaves()
        {
            enabled = false;
        }

        void Update()
        {
            if (_wavesCompleted)
            {
                StopWaves();
                m_completeEvent.Invoke();
                return;
            }

            if (m_activeEnemies.Count < m_maxSpawnCount) //there is room for more enemies to spawn
                if (m_waveDelayCountdown <= 0 && m_spawnRateCountdown <= 0) //check countdowns
                {
                    for (int i = 0; i < m_batchSpawn; i++) //attempt to spawn a batch of enemies
                    {
                        Enemy newEnemy = m_currentWave.AttemptSpawnEnemy(transform.position);
                        if (newEnemy)
                        {
                            //other spawn functions
                            m_spawnRateCountdown = m_spawnRate; //reset countdown
                            m_activeEnemies.Add(newEnemy); //save to list
                            newEnemy.deathCallback = RemoveEnemyFromActiveList;
                            m_particles.PlayParticle(m_spawnParticleName, newEnemy.transform);
                        }
                        if (m_activeEnemies.Count >= m_maxSpawnCount && !m_canBatchOverflow) break;
                    }

                    if (m_activeEnemies.Count == 0) //there are no enemies have been spawned and there are no more living, move on to the next wave
                    {
                        m_waveIndex++;
                        m_waveDelayCountdown = m_waveDelay;
                    }
                }
            if (m_waveDelayCountdown > 0) m_waveDelayCountdown -= Time.deltaTime; //wave delay
            if (m_spawnRateCountdown > 0) m_spawnRateCountdown -= Time.deltaTime; //spawn rate
        }
        private void RemoveEnemyFromActiveList(Enemy deadGuy)
        {
            if (m_activeEnemies.Contains(deadGuy))
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