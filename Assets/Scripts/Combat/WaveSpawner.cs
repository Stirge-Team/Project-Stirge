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
        public class WaveEnemyData
        {
            [SerializeField]
            private Enemy m_type;
            [SerializeField]
            private int m_maxCount = 1;
            private int m_spawnedCount;
            public bool _spawnsAvalible => m_spawnedCount < m_maxCount;
            public Enemy SpawnEnemy(Vector3 pos)
            {
                Enemy newEnemy = Instantiate(m_type, pos, Quaternion.identity);
                newEnemy.name = m_type.name;
                m_spawnedCount++;
                return newEnemy;
            }
        }
        [System.Serializable]
        public class Wave
        {
            [SerializeField] //x,y are the center point, z,w are the bounds. i see *no* issue with this
            private Vector4 m_spawnArea = Vector4.one;
            public Vector4 SpawnArea { get { return m_spawnArea; } set { } }
            [SerializeField]
            private float m_startDelay = 1f;
            private bool m_delayComplete => m_startDelay <= 0;
            [SerializeField]
            private float m_spawnRate = 0.5f;
            private float m_spawnRateCountdown;
            private bool m_spawnCountdownComplete => m_spawnRateCountdown <= 0;
            [SerializeField, Tooltip("How many enemies (within the maximum) should be spawned at once.")]
            private int m_batchSpawn = 1;
            [SerializeField, Tooltip("Can the number of enemies exceed the maximum limit if spawning a batch of enemies would cause it to do so. i.e. enemy limit is 4, spawning in batches of 3. First batch spawns in, then the second resulting in 6 enemies rather then 4.")]
            private bool m_canBatchOverflow = false;
            [SerializeField]
            private int m_maxSpawnCount = 3;
            [SerializeField, Tooltip("The list of enemies to spawn. They will be spawned in the order listed.")]
            private WaveEnemyData[] m_enemyList;
            private int m_spawnIndex;
            public bool _outOfSpawns => m_spawnIndex >= m_enemyList.Count();
            public void Init()
            {
                m_spawnRateCountdown = m_spawnRate;
                m_spawnIndex = 0;
                if (m_batchSpawn > m_maxSpawnCount && m_canBatchOverflow) Debug.LogWarning("Enemy batch spawn count exceeds the maximum enemy count. Please keep the Batch Spawn value less then or equal to the max limit unless this was intened.");
            }
            public Enemy[] AttemptSpawnEnemy(Vector3 origin, WaveSpawner spawner)
            {
                if (m_delayComplete && m_spawnCountdownComplete && !_outOfSpawns && spawner._activeEnemies.Count() < m_maxSpawnCount) //check countdowns
                {
                    Enemy[] batch = new Enemy[m_batchSpawn];
                    for (int x = 0; x < m_batchSpawn; x++) //spawn the batch amount
                    {
                        if(spawner._activeEnemies.Count() + x >= m_maxSpawnCount && !m_canBatchOverflow) break;
                        
                        if (m_enemyList[m_spawnIndex]._spawnsAvalible) //check for any avalible spawns
                        {

                            //spawning position code
                            float rndXBound = Random.Range(-m_spawnArea.z / 2, m_spawnArea.z / 2);
                            float rndZBound = Random.Range(-m_spawnArea.w / 2, m_spawnArea.w / 2);
                            Vector3 position = origin + new Vector3(m_spawnArea.x, 0, m_spawnArea.y) + new Vector3(rndXBound, 0, rndZBound); //origin + set offset + rnd offset within range

                            //TODO: Add player avoidance spawn code so that enemies don't spawn on the player

                            batch[x] = m_enemyList[m_spawnIndex].SpawnEnemy(position); //instance the enemy and save
                            m_spawnRateCountdown = m_spawnRate; //reset countdown
                            if (!m_enemyList[m_spawnIndex]._spawnsAvalible) m_spawnIndex++; //increase the spawn index if all the avalible enemies of that type have spawned.

                        }
                        
                    }
                    return batch;
                }

                if (!m_delayComplete) m_startDelay -= Time.deltaTime;
                if (!m_spawnCountdownComplete) m_spawnRateCountdown -= Time.deltaTime;

                Enemy[] mt = new Enemy[0];
                return mt;
            }
        }

        [SerializeField]
        private Wave[] m_waves;
        private int m_waveIndex = 0;
        private Wave m_currentWave => m_waves[m_waveIndex];
        public bool _wavesCompleted { get { return m_waveIndex >= m_waves.Count(); } }
        [SerializeField]
        private UnityEvent m_completeEvent;

        private List<Enemy> m_activeEnemies = new();
        public List<Enemy> _activeEnemies { get { return m_activeEnemies; } }
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
            m_waveIndex = 0;
            m_currentWave.Init();
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

            if (m_activeEnemies.Count == 0 && m_currentWave._outOfSpawns) //there are no enemies to be spawned and there are no more living, move on to the next wave
            {
                m_waveIndex++;
                m_currentWave.Init();
            }

            Enemy[] enemyBatch = m_currentWave.AttemptSpawnEnemy(transform.position, this);
            foreach (var newEnemy in enemyBatch)
            {
                if (newEnemy)
                {
                    //other spawn functions
                    m_activeEnemies.Add(newEnemy); //save to list
                    newEnemy.deathCallback = RemoveEnemyFromActiveList;
                    m_particles.PlayParticle(m_spawnParticleName, newEnemy.transform);
                }
            }

        }
        private void RemoveEnemyFromActiveList(Enemy deadGuy)
        {
            if (m_activeEnemies.Contains(deadGuy))
                m_activeEnemies.Remove(deadGuy);
        }

        void OnDrawGizmos()
        {
            if(m_waves != null) foreach (var wave in m_waves)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, m_gizmoAlpha);
                Gizmos.DrawCube(transform.position + new Vector3(wave.SpawnArea.x, 0, wave.SpawnArea.y), new(wave.SpawnArea.z, 1, wave.SpawnArea.w));
            }
        }
    }
}