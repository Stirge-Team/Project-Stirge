using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public sealed class EnemyBlackboard : MonoBehaviour
    {
        [SerializeField] private UtilityEnemy m_enemy;

        private readonly Dictionary<Type, IBlackboardTable> m_tables = new();
        private readonly Dictionary<string, ValueIndex> m_properties = new();

        public UtilityEnemy Enemy => m_enemy;

        private void Start()
        {
            Init(m_enemy);
        }

        public void Init(UtilityEnemy enemy)
        {
            m_enemy = enemy;
        }

        #region ValueGetters
        public bool TryGetStructValue<T>(string propertyName, out T value) where T : struct
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || valueIndex.table.valueType != typeof(T))
            {
                value = default;
                //throw new Exception("No Value exists with name {0}. Make sure you're using a constant from EnemyConstants.cs.");
                return false;
            }

            var table = (BlackboardTable<T>)valueIndex.table;
            value = table.GetValue(valueIndex.index);

            return true;
        }

        public bool TryGetClassValue<T>(string propertyName, out T value) where T : class
        {
            bool answer = TryGetObjectValue(typeof(T), propertyName, out object objectValue);
            value = objectValue as T;

            return answer;
        }

        public bool TryGetObjectValue(Type valueType, string propertyName, out object value)
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

        public bool TryGetObjectValue(string propertyName, out object value)
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                value = default;
                return false;
            }

            IBlackboardTable table = valueIndex.table;
            value = table.GetObjectValue(valueIndex.index);

            return true;
        }
        #endregion

        #region ValueSetters
        public void SetStructValue<T>(string propertyName, T value) where T : struct
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
                    valueIndex.table.RemoveAt(valueIndex.index);
                    BlackboardTable<T> table = GetOrCreateTable<T>();
                    int index = table.AddValue(value);
                    m_properties[propertyName] = new ValueIndex(table, index);
                }
            }
            else
            {
                BlackboardTable<T> table = GetOrCreateTable<T>();
                int index = table.AddValue(value);
                m_properties[propertyName] = new ValueIndex(table, index);
            }
        }

        public void SetClassValue<T>(string propertyName, T value) where T : class
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
                    valueIndex.table.RemoveAt(valueIndex.index);
                    IBlackboardTable table = GetOrCreateTable(valueType);
                    int index = table.AddObjectValue(value);
                    m_properties[propertyName] = new ValueIndex(table, index);
                }
            }
            else
            {
                IBlackboardTable table = GetOrCreateTable(valueType);
                int index = table.AddObjectValue(value);
                m_properties[propertyName] = new ValueIndex(table, index);
            }
        }

        public void SetObjectValue(Type valueType, string propertyName, object value)
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
                    valueIndex.table.RemoveAt(valueIndex.index);
                    IBlackboardTable table = GetOrCreateTable(valueType);
                    int index = table.AddObjectValue(value);
                    m_properties[propertyName] = new ValueIndex(table, index);
                }
            }
            else
            {
                IBlackboardTable table = GetOrCreateTable(valueType);
                int index = table.AddObjectValue(value);
                m_properties[propertyName] = new ValueIndex(table, index);
            }
        }
        #endregion

        #region ValueRemovers
        public bool RemoveStruct<T>(string propertyName) where T : struct
        {
            Type valueType = typeof(T);

            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || valueIndex.table.valueType != valueType)
            {
                return false;
            }

            m_properties.Remove(propertyName);
            var typedTable = (BlackboardTable<T>)valueIndex.table;
            typedTable.RemoveAt(valueIndex.index);

            return true;
        }
        public bool RemoveObject<T>(string propertyName)
        {
            Type valueType = typeof(T);

            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || !valueType.IsAssignableFrom(valueIndex.table.valueType))
            {
                return false;
            }

            m_properties.Remove(propertyName);
            valueIndex.table.RemoveAt(valueIndex.index);

            return true;
        }
        public bool RemoveObject(Type valueType, string propertyName)
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || !valueType.IsAssignableFrom(valueIndex.table.valueType))
            {
                return false;
            }

            m_properties.Remove(propertyName);
            valueIndex.table.RemoveAt(valueIndex.index);

            return true;
        }
        public bool RemoveObject(string propertyName)
        {
            if (!m_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                return false;
            }

            m_properties.Remove(propertyName);
            valueIndex.table.RemoveAt(valueIndex.index);

            return true;
        }
        #endregion

        #region Clear
        public void Clear()
        {
            m_properties.Clear();

            foreach (IBlackboardTable table in m_tables.Values)
            {
                table.Clear();
            }
        }
        #endregion

        #region Tables
        private IBlackboardTable GetOrCreateTable(Type valueType)
        {
            if (!m_tables.TryGetValue(valueType, out IBlackboardTable table))
            {
                table = CreateTable(valueType);
            }

            return table;
        }
        private IBlackboardTable CreateTable(Type valueType)
        {
            Type tableType = typeof(BlackboardTable<>).MakeGenericType(valueType);

            var table = (IBlackboardTable)Activator.CreateInstance(tableType);
            m_tables.Add(valueType, table);

            return table;
        }

        private BlackboardTable<T> GetOrCreateTable<T>()
        {
            BlackboardTable<T> answer = m_tables.TryGetValue(typeof(T), out IBlackboardTable table)
                ? (BlackboardTable<T>)table
                : CreateTable<T>();

            return answer;
        }
        private BlackboardTable<T> CreateTable<T>()
        {
            Type valueType = typeof(T);

            var table = new BlackboardTable<T>();
            m_tables.Add(valueType, table);

            return table;
        }

        private readonly struct ValueIndex
        {
            public readonly IBlackboardTable table;
            public readonly int index;

            public ValueIndex(IBlackboardTable table, int index)
            {
                this.table = table;
                this.index = index;
            }
        }
        #endregion
    }
}
