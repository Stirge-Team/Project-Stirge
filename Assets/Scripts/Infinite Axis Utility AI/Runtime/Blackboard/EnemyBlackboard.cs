using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    using Core;
    using UnityEditor.ShaderGraph.Internal;
    using static UnityEditor.Search.SearchValue;

    public class EnemyBlackboard
    {
        #region Static Setup
        public static readonly PropertyInfo[] CachedPropertyInfosArray = typeof(UtilityEnemy).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly Dictionary<Type, IBlackboardTable> m_tables = new();
        private static readonly Dictionary<BlackboardPropertyName, ValueIndex> m_properties = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            Dictionary<Type, PropertyInfo[]> propertyInfosByType = new();

            foreach (PropertyInfo info in CachedPropertyInfosArray)
            {
                Type propertyType = info.PropertyType;
                if (!propertyInfosByType.ContainsKey(propertyType))
                {
                    propertyInfosByType.Add(propertyType, CachedPropertyInfosArray.Where(info => info.PropertyType == propertyType).ToArray());
                }
            }

            foreach (var e in propertyInfosByType)
            {
                Type propertyType = e.Key;
                PropertyInfo[] propertyInfos = e.Value;

                // Create a new Table
                Type tableType = typeof(BlackboardTable<>).MakeGenericType(propertyType);
                var table = (IBlackboardTable)Activator.CreateInstance(tableType);
                m_tables.Add(propertyType, table);
                table.Setup(propertyInfos);

                // Cache properties
                for (int i = 0, count = propertyInfos.Length; i < count; i++)
                {
                    PropertyInfo info = propertyInfos[i];
                    m_properties.Add(new BlackboardPropertyName(info.Name), new ValueIndex(table, i));
                }
            }
        }
        #endregion

        public EnemyBlackboard(UtilityEnemy enemy)
        {
            m_enemy = enemy;
        }

        private UtilityEnemy m_enemy;

        #region Get
        public bool TryGetStructValue<T>(BlackboardPropertyName propertyName, out T value) where T : struct
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || valueIndex.table.valueType != typeof(T))
            {
                value = default;
                return false;
            }

            var table = (BlackboardTable<T>)valueIndex.table;
            value = table.GetValue(m_enemy, valueIndex.index);
            return true;
        }
        public bool TryGetClassValue<T>(BlackboardPropertyName propertyName, out T value) where T : class
        {
            bool answer = TryGetObjectValue(typeof(T), propertyName, out object objectValue);
            value = objectValue as T;
            return answer;
        }
        public bool TryGetObjectValue(Type valueType, BlackboardPropertyName propertyName, out object value)
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || !valueType.IsAssignableFrom(valueIndex.table.valueType))
            {
                value = default;
                return false;
            }

            IBlackboardTable table = valueIndex.table;
            value = table.GetObjectValue(m_enemy, valueIndex.index);
            return true;
        }
        #endregion

        #region Set
        public void SetStructValue<T>(BlackboardPropertyName propertyName, T value) where T : struct
        {
            Type newType = typeof(T);

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == newType)
                {
                    var table = (BlackboardTable<T>)valueIndex.table;
                    table.SetValue(m_enemy, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Table of Type '{newType.Name}' does not exist on {nameof(UtilityEnemy)}!");
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{newType.Name}' does not exist on {nameof(UtilityEnemy)}!");
            }
        }
        public void SetClassValue<T>(BlackboardPropertyName propertyName, T value) where T : class
        {
            Type valueType = value == null ? typeof(T) : value.GetType();

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    IBlackboardTable table = valueIndex.table;
                    table.SetObjectValue(m_enemy, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Table of Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!");
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!");
            }
        }
        public void SetObjectValue(Type valueType, BlackboardPropertyName propertyName, object value)
        {
            if (value != null)
            {
                valueType = value.GetType();
            }

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    IBlackboardTable table = m_tables[valueType];
                    table.SetObjectValue(m_enemy, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Property with Name '{propertyName.Name}' exists with Type '{valueIndex.table.valueType}', not Type '{valueType}' on {nameof(UtilityEnemy)}!");
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!");
            }
        }
        #endregion

        private struct ValueIndex
        {
            public readonly IBlackboardTable table;
            public readonly int index;

            public ValueIndex(IBlackboardTable table, int index)
            {
                this.table = table;
                this.index = index;
            }
        }
    }
}
