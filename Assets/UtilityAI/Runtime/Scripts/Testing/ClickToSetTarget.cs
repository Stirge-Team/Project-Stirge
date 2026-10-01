using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Stirge.UtilityAI.Testing
{
    using Combat;

    public class ClickToSetTarget : MonoBehaviour
    {
        [SerializeField] private List<UtilityEnemy> m_enemies = new();

        private UnityEngine.Camera m_camera;
        private Ray m_lastRay;

        private void Awake()
        {
            m_camera = UnityEngine.Camera.main;
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
                Ray ray = m_camera.ScreenPointToRay(mouseScreenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // First check for a new target
                    Rigidbody hitRigidbody = hit.rigidbody;
                    if (hitRigidbody != null && hitRigidbody.transform.parent != null)
                    {
                        CombatEntity newTarget = hitRigidbody.transform.parent.GetComponent<CombatEntity>();
                        SetTargetOfEnemies(newTarget);
                        return;
                    }

                    // Then check for NavMesh point
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit meshHit, 1f, NavMesh.AllAreas))
                    {
                        Vector3 newTargetPosition = meshHit.position;
                        SetTargetPositionOfEnemies(newTargetPosition);
                        return;
                    }

                    // If no target was found, then clear targets
                    ClearEnemyTargets();
                }
            }
        }

        private void SetTargetOfEnemies(CombatEntity newTarget)
        {
            foreach (UtilityEnemy enemy in m_enemies)
            {
                enemy.SetTarget(newTarget);
            }
        }

        private void SetTargetPositionOfEnemies(Vector3 newTargetPosition)
        {
            foreach (UtilityEnemy enemy in m_enemies)
            {
                enemy.SetTargetPosition(newTargetPosition);
            }
        }

        private void ClearEnemyTargets()
        {
            foreach (UtilityEnemy enemy in m_enemies)
            {
                enemy.ClearTarget();
            }
        }

        private void OnDrawGizmos()
        {
            // if LastRay is not default (structs cannot be null)
            if (!m_lastRay.Equals(default(Ray)))
            {
                Gizmos.DrawLine(m_camera.transform.position, m_lastRay.origin + m_lastRay.direction * 30f);
            }
        }
    }
}
