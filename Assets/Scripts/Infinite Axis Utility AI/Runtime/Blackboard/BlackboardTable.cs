using System;
using System.Reflection;
using UnityEngine;

// https://www.youtube.com/watch?v=er9nD-usM1A&t=588s
// https://www.reddit.com/r/csharp/comments/tz0jso/why_is_reflection_faster_using_delegates/

namespace Stirge.UtilityAI.Blackboard
{
    public sealed class BlackboardTable<T> : IBlackboardTable
    {
        private BlackboardTableProperty[] m_values;

        public Type valueType => typeof(T);
        public int count => m_values.Length;

        public void Setup(PropertyInfo[] propertyInfos)
        {
            int count = propertyInfos.Length;
            m_values = new BlackboardTableProperty[count];
            for (int i = 0; i < count; i++)
            {
                PropertyInfo info = propertyInfos[i];
                m_values[i] = new(
                    info.Name,
                    (Func<UtilityEnemy, T>) Delegate.CreateDelegate(typeof(Func<UtilityEnemy, T>), info.GetGetMethod()),
                    (Action<UtilityEnemy, T>) Delegate.CreateDelegate(typeof(Action<UtilityEnemy, T>), info.GetSetMethod())
                );
            }
        }
        
        public T GetValue(UtilityEnemy enemy, int index)
        {
            return m_values[index].getMethod(enemy);
        }
        public object GetObjectValue(UtilityEnemy enemy, int index)
        {
            return GetValue(enemy, index);
        }

        public void SetValue(UtilityEnemy enemy, T value, int index)
        {
            BlackboardTableProperty property = m_values[index];
            if (property.setMethod != null)
                property.setMethod(enemy, value);
            else
                Debug.LogError($"Property with Name '{property.propertyName}' is read-only and/or does not have a defined Set method!");
        }

        public void SetObjectValue(UtilityEnemy enemy, object value, int index)
        {
            SetValue(enemy, (T)value, index);
        }

        private readonly struct BlackboardTableProperty
        {
            public readonly string propertyName;
            public readonly Func<UtilityEnemy, T> getMethod;
            public readonly Action<UtilityEnemy, T> setMethod;

            public BlackboardTableProperty(string name, Func<UtilityEnemy, T> getMethod, Action<UtilityEnemy, T> setMethod)
            {
                propertyName = name;
                this.getMethod = getMethod;
                this.setMethod = setMethod;
            }
        }
    }
}
