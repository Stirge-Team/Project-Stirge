using UnityEngine;
using System;

namespace Stirge.UtilityAI.Blackboard
{
    using Enemy;
    using Tools;

    public sealed class EnemyBlackboard : MonoBehaviour
    {
        [SerializeField] private UtilityEnemy m_enemy;
        [SerializeField] private EnemyBlackboardFloat m_floatDict = new();
        [SerializeField] private EnemyBlackboardVector3 m_vector3Dict = new();

        public UtilityEnemy Enemy => m_enemy;

        private void Start()
        {
            Init(m_enemy);
        }

        public void Init(UtilityEnemy enemy)
        {
            m_enemy = enemy;
            m_floatDict[EnemyConstants.IsGrounded] = () => m_enemy.IsGrounded() ? 1 : 0;
            m_floatDict[EnemyConstants.GetSpeed] = () => m_enemy.GetMovementSpeed();

            m_vector3Dict[EnemyConstants.GetPosition] = () => m_enemy.GetPosition();
            m_vector3Dict[EnemyConstants.GetEulerRotation] = () => m_enemy.GetEulerRotation();
        }

        #region GetValue
        public float GetFloatValue(string delegateName)
        {
            if (m_floatDict.TryGetValue(delegateName, out Func<float> valueDelegate))
            {
                return valueDelegate();
            }
            else
            {
                throw new Exception("No Delegate exists with name {0}. Make sure you're using a constant from EnemyConstants.cs.");
            }
        }

        public Vector3 GetVector3Value(string delegateName)
        {
            if (m_vector3Dict.TryGetValue(delegateName, out Func<Vector3> valueDelegate))
            {
                return valueDelegate();
            }
            else
            {
                throw new Exception("No Delegate exists with name {0}. Make sure you're using a constant from EnemyConstants.cs.");
            }
        }
        #endregion

        #region Dictionaries
        [Serializable]
        private class EnemyBlackboardFloat : SerializableDictionary<string, Func<float>>
        {
            public EnemyBlackboardFloat()
            {
                Add(EnemyConstants.IsGrounded, default);
                Add(EnemyConstants.GetSpeed, default);
            }
        }
        [Serializable]
        private class EnemyBlackboardVector3 : SerializableDictionary<string, Func<Vector3>>
        {
            public EnemyBlackboardVector3()
            {
                Add(EnemyConstants.GetPosition, default);
                Add(EnemyConstants.GetEulerRotation, default);
            }

        }
        #endregion
    }
}
