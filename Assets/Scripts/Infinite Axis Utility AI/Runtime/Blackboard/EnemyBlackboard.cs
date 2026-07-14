using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    using Core;

    public class EnemyBlackboard
    {
        public static readonly PropertyInfo[] CachedPropertyInfosArray = typeof(UtilityEnemy).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly Dictionary<Type, IBlackboardTable> m_tables = new();
        private static readonly Dictionary<BlackboardPropertyName, ValueIndex> m_properties = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            foreach (var info in CachedPropertyInfosArray)
            {
                Type propertyType = info.PropertyType;
                int propertyHash = info.Name.GetHashCode();

                // try get table of type
                m_tables.TryGetValue(propertyType, out var table);
                table ??= new();
                table.Add(propertyHash, (Func<UtilityEnemy, object>)Delegate.CreateDelegate(typeof(Func<UtilityEnemy, object>), info.GetGetMethod()!));
            }
        }

        public bool TryGetStructValue<T>(BlackboardPropertyName propertyName, out T value) where T : struct
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || valueIndex.table.valueType != typeof(T))
            {
                value = default;
                return false;
            }

            var table = (BlackboardTable<T>)valueIndex.table;
            value = table.GetValue(valueIndex.index);
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
            value = table.GetObjectValue(valueIndex.index);
            return true;
        }

        public void SetStructValue<T>(BlackboardPropertyName propertyName, T value) where T : struct
        {
            Type newType = typeof(T);

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == newType)
                {
                    var table = (BlackboardTable<T>)valueIndex.table;
                    table.SetValue(value, valueIndex.index);
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
                    table.SetObjectValue(value, valueIndex.index);
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
                    table.SetObjectValue(value, valueIndex.index);
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
