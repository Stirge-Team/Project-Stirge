using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stirge.Player
{
    //Stirge namespaces
    using Camera;
    using Enemy;

    public class EnemyToCamPasser : MonoBehaviour
    {
        private List<Transform> m_enemyAgentList = new List<Transform>();

        [SerializeField]
        private float m_range = 10;

        [Header("Function update times")]
        [SerializeField]
        private float m_GrabEnemiesWaitTime = 1;

        [SerializeField]
        private float m_ValidateWaitTime = 2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(GrabAllEnemies(m_GrabEnemiesWaitTime));
            StartCoroutine(ValidateEnemies(m_ValidateWaitTime));
        }

        private IEnumerator GrabAllEnemies(float waitTime)
        {
            m_enemyAgentList.Clear();
            foreach (var ene in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                m_enemyAgentList.Add(ene.GetComponentInChildren<NavMeshAgent>().transform);
            }
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(GrabAllEnemies(waitTime));
        }

        private IEnumerator ValidateEnemies(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (m_enemyAgentList.Count > 0)
            {
                List<Transform> validEnemies = new();
                foreach (var ene in m_enemyAgentList)
                {
                    if (ene == null)
                        continue;
                    float dist = Vector3.Distance(ene.position, transform.position);
                    if (dist <= m_range)
                    {
                        validEnemies.Add(ene);
                    }
                }
                FindFirstObjectByType<TrackingCamera>()
                    .ReplaceSecondaryTargets(validEnemies.ToArray());
            }
            StartCoroutine(ValidateEnemies(waitTime));
        }
    }
}
