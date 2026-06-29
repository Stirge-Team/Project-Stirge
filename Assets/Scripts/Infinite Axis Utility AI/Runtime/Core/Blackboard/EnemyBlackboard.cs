using System;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    using Tools;

    public sealed class EnemyBlackboard : MonoBehaviour
    {
        [SerializeField] private UtilityEnemy m_enemy;
        [SerializeField] private SerializableDictionary<string, ScoreCallback> m_callbacks;

        public void Init(UtilityEnemy enemy)
        {
            m_enemy = enemy;
        }

        public float GetValue(string methodName)
        {
            if (m_callbacks.TryGetValue(methodName, out ScoreCallback callback))
                return callback.Invoke();
            else
                throw new Exception("No Callback exists with name {0}. Make sure you're using a constant from EnemyConstants.cs.");
        }
    }
}