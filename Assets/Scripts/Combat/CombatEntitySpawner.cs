using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Stirge.Combat
{
    public class CombatEntitySpawner : MonoBehaviour
    {
        [SerializeField] private CombatEntity m_combatEntityPrefab;
        [SerializeField, Min(0)] private int m_targetSpawnCount;
        [SerializeField] private Transform m_spawnLocation;

        private List<CombatEntity> m_spawnedEntities;

        private void Start()
        {
            m_spawnedEntities = new();
            for (int i = 0; i < m_targetSpawnCount; i++)
                SpawnCombatEntity();
        }

        private void SpawnCombatEntity()
        {
            CombatEntity spawnedEntity = Instantiate(m_combatEntityPrefab, m_spawnLocation.position, m_spawnLocation.rotation);
            spawnedEntity.spawner = this;
            spawnedEntity.name = m_combatEntityPrefab.name;
            m_spawnedEntities.Add(spawnedEntity);
        }

        public void ReportDeath(CombatEntity enemy)
        {
            m_spawnedEntities.Remove(enemy);
            SpawnCombatEntity();
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
                foreach (CombatEntity entity in m_spawnedEntities)
                {
                    entity.EnterStun(3f);
                }
            }
        }
        public void DebugKnockback(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (CombatEntity entity in m_spawnedEntities)
                {
                    entity.EnterKnockback(10f, new Vector2(1, 1), 1.3f, 0);
                }
            }
        }
        public void DebugAirJuggle(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (CombatEntity entity in m_spawnedEntities)
                {
                    entity.EnterAirJuggle(6f, Vector3.up, 1.3f, 0);
                }
            }
        }
        public void DebugReduceHealth(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (CombatEntity entity in m_spawnedEntities)
                {
                    entity.TakeDamage(1);
                }
            }
        }
#endif
    }
}
