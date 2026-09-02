using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stirge.GenericBlackboard
{
    public static class GenericBlackboard<TBase> where TBase : MonoBehaviour
    {
        static GenericBlackboard()
        {
            CachedPropertyInfosArray = typeof(TBase).GetProperties(s_propertyFlags);

            // Check each PropertyInfo in the cache and organise them into this dictionary by Type
            // Check all of them to ensure we create a new entry in the dictionary for every unique Type
            Dictionary<Type, PropertyInfo[]> propertyInfosByType = new();

            foreach (PropertyInfo info in CachedPropertyInfosArray)
            {
                Type propertyType = info.PropertyType;
                if (!propertyInfosByType.ContainsKey(propertyType))
                {
                    propertyInfosByType.Add(propertyType, CachedPropertyInfosArray.Where(info => info.PropertyType == propertyType).ToArray());
                }
            }

            // Go through each element/entry/key value pair in the dictionary
            // Each entry consists of:
            // - a Type
            // - an array of PropertyInfos that are all of the above Type
            foreach (var e in propertyInfosByType)
            {
                Type propertyType = e.Key;
                PropertyInfo[] propertyInfos = e.Value;

                // Create a new Table for this Type
                Type tableType = typeof(BlackboardTable<,>).MakeGenericType(typeof(TBase), propertyType);
                var table = (IBlackboardTable<TBase>)Activator.CreateInstance(tableType);
                s_tables.Add(propertyType, table);
                table.Setup(propertyInfos);

                // Add properties to s_properties Dictionary
                for (int i = 0, count = propertyInfos.Length; i < count; i++)
                {
                    PropertyInfo info = propertyInfos[i];
                    s_properties.Add(new BlackboardPropertyName(info.Name, propertyType), new ValueIndex(table, i));
                }
            }
        }
        
        private static readonly BindingFlags s_propertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public static readonly PropertyInfo[] CachedPropertyInfosArray;

        private static readonly Dictionary<Type, IBlackboardTable<TBase>> s_tables = new();
        private static readonly Dictionary<BlackboardPropertyName, ValueIndex> s_properties = new();

        #region Get
        public static bool TryGetStructValue<TValue>(TBase target, BlackboardPropertyName propertyName, out TValue value) where TValue : struct
        {
            if (!s_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || valueIndex.table.valueType != typeof(TValue))
            {
                value = default;
                return false;
            }

            var table = (BlackboardTable<TBase, TValue>)valueIndex.table;
            value = table.GetValue(target, valueIndex.index);
            return true;
        }
        public static bool TryGetClassValue<TValue>(TBase target, BlackboardPropertyName propertyName, out TValue value) where TValue : class
        {
            bool answer = TryGetObjectValue(target, typeof(TValue), propertyName, out object objectValue);
            value = objectValue as TValue;
            return answer;
        }
        public static bool TryGetObjectValue(TBase target, Type valueType, BlackboardPropertyName propertyName, out object value)
        {
            if (!s_properties.TryGetValue(propertyName, out ValueIndex valueIndex) || !valueType.IsAssignableFrom(valueIndex.table.valueType))
            {
                value = default;
                return false;
            }

            IBlackboardTable<TBase> table = valueIndex.table;
            value = table.GetObjectValue(target, valueIndex.index);
            return true;
        }
        #endregion

        #region Set
        public static void SetStructValue<TValue>(TBase target, BlackboardPropertyName propertyName, TValue value) where TValue : struct
        {
            Type valueType = typeof(TValue);

            if (s_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    var table = (BlackboardTable<TBase, TValue>)valueIndex.table;
                    table.SetValue(target, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Table of Type '{valueType.Name}' does not exist on {nameof(TValue)}!", target);
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {typeof(TBase).Name}!", target);
            }
        }
        public static void SetClassValue<TValue>(TBase target, BlackboardPropertyName propertyName, TValue value) where TValue : class
        {
            Type valueType = value == null ? typeof(TValue) : value.GetType();

            if (s_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    IBlackboardTable<TBase> table = valueIndex.table;
                    table.SetObjectValue(target, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Table of Type '{valueType.Name}' does not exist on {typeof(TBase).Name}!", target);
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {typeof(TBase).Name}!", target);
            }
        }
        public static void SetObjectValue(TBase target, Type valueType, BlackboardPropertyName propertyName, object value)
        {
            if (value != null)
            {
                valueType = value.GetType();
            }

            if (s_properties.TryGetValue(propertyName, out ValueIndex valueIndex))
            {
                if (valueIndex.table.valueType == valueType)
                {
                    IBlackboardTable<TBase> table = s_tables[valueType];
                    table.SetObjectValue(target, value, valueIndex.index);
                }
                else
                {
                    Debug.LogError($"Property with Name '{propertyName.Name}' exists with Type '{valueIndex.table.valueType}', not Type '{valueType}' on {typeof(TBase).Name}!", target);
                }
            }
            else
            {
                Debug.LogError($"Property with Name '{propertyName.Name}' with Type '{valueType.Name}' does not exist on {typeof(TBase).Name}!", target);
            }
        }
        #endregion

        /// <summary>
        /// Reference to a property in an <see cref="IBlackboardTable{TBase}"/>.<br/>
        /// '<see cref="ValueIndex.table"/>' is a reference to the <see cref="IBlackboardTable{TBase}"/> the property is in.<br/>
        /// '<see cref="ValueIndex.index"/>' is the index the property resides at in the table.
        /// </summary>
        private readonly struct ValueIndex
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
