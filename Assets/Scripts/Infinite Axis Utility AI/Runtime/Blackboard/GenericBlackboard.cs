using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public class GenericBlackboard<TBase> : GenericBlackboard_Base where TBase : MonoBehaviour
    {
        #region Static Setup
        public static readonly PropertyInfo[] CachedPropertyInfosArray = typeof(TBase).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        protected static readonly Dictionary<Type, IBlackboardTable<TBase>> m_tables = new();
        protected static readonly Dictionary<BlackboardPropertyName, ValueIndex> m_properties = new();

        protected static void Setup()
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
                Type tableType = typeof(BlackboardTable<,>).MakeGenericType(typeof(TBase), propertyType);
                var table = (IBlackboardTable<TBase>)Activator.CreateInstance(tableType);
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

        public sealed override PropertyInfo[] GetCachedPropertyInfosArray => CachedPropertyInfosArray;

        public GenericBlackboard() { }
        public GenericBlackboard(TBase target)
        {
            m_target = target;
        }

        private TBase m_target;

        #region Get
        public override bool TryGetStructValue<TValue>(BlackboardPropertyName propertyName, out TValue value) where TValue : struct
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || valueIndex.table.valueType != typeof(TValue))
            {
                value = default;
                return false;
            }

            var table = (BlackboardTable<TBase, TValue>)valueIndex.table;
            value = table.GetValue(m_target, valueIndex.index);
            return true;
        }
        public override bool TryGetClassValue<TValue>(BlackboardPropertyName propertyName, out TValue value) where TValue : class
        {
            bool answer = TryGetObjectValue(typeof(TValue), propertyName, out object objectValue);
            value = objectValue as TValue;
            return answer;
        }
        public override bool TryGetObjectValue(Type valueType, BlackboardPropertyName propertyName, out object value)
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || !valueType.IsAssignableFrom(valueIndex.table.valueType))
            {
                value = default;
                return false;
            }

            IBlackboardTable<TBase> table = valueIndex.table;
            value = table.GetObjectValue(m_target, valueIndex.index);
            return true;
        }
        #endregion

        #region Set
        public override void SetStructValue<TValue>(BlackboardPropertyName propertyName, TValue value) where TValue : struct
        {
            Type valueType = typeof(TValue);

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    var table = (BlackboardTable<TBase, TValue>)valueIndex.table;
                    table.SetValue(m_target, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Table of Type '{valueType.Name}' does not exist on {nameof(TValue)}!", m_target);
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!", m_target);
            }
        }
        public override void SetClassValue<TValue>(BlackboardPropertyName propertyName, TValue value) where TValue : class
        {
            Type valueType = value == null ? typeof(TValue) : value.GetType();

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    IBlackboardTable<TBase> table = valueIndex.table;
                    table.SetObjectValue(m_target, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Table of Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!", m_target);
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!", m_target);
            }
        }
        public override void SetObjectValue(Type valueType, BlackboardPropertyName propertyName, object value)
        {
            if (value != null)
            {
                valueType = value.GetType();
            }

            if (m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    IBlackboardTable<TBase> table = m_tables[valueType];
                    table.SetObjectValue(m_target, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Property with Name '{propertyName.Name}' exists with Type '{valueIndex.table.valueType}', not Type '{valueType}' on {nameof(UtilityEnemy)}!", m_target);
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {nameof(UtilityEnemy)}!", m_target);
            }
        }
        #endregion

        protected struct ValueIndex
        {
            public readonly IBlackboardTable<TBase> table;
            public readonly int index;

            public ValueIndex(IBlackboardTable<TBase> table, int index)
            {
                this.table = table;
                this.index = index;
            }
        }
    }
}
