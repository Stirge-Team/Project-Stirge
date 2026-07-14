using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// https://www.youtube.com/watch?v=er9nD-usM1A&t=588s
// https://www.reddit.com/r/csharp/comments/tz0jso/why_is_reflection_faster_using_delegates/

namespace Stirge.UtilityAI.Blackboard
{
    public class BlackboardTable<T> : IBlackboardTable
    {
        private UtilityEnemy m_enemy;
        private BlackboardTableProperty[] m_values;

        public Type valueType => typeof(T);

        public void Setup(UtilityEnemy enemy, PropertyInfo[] propertyInfos)
        {
            m_enemy = enemy;
            int count = propertyInfos.Length;
            m_values = new BlackboardTableProperty[count];
            for (int i = 0; i < count; i++)
            {
                PropertyInfo info = propertyInfos[i];
                m_values[i] = new((Func<UtilityEnemy, T>) Delegate.CreateDelegate(typeof(Func<UtilityEnemy, T>), info.GetGetMethod()),
                                (Action<UtilityEnemy, T>) Delegate.CreateDelegate(typeof(Action<UtilityEnemy, T>), info.GetSetMethod()));
            }
        }
        
        public T GetValue(int index)
        {
            return m_values[index].getMethod(m_enemy);
        }
        public object GetObjectValue(int index)
        {
            return GetValue(index);
        }

        public void SetValue(T value, int index)
        {
            m_values[index].setMethod(m_enemy, value);
        }

        public void SetObjectValue(object value, int index)
        {
            SetValue((T)value, index);
        }

        private readonly struct BlackboardTableProperty
        {
            public readonly Func<UtilityEnemy, T> getMethod;
            public readonly Action<UtilityEnemy, T> setMethod;

            public BlackboardTableProperty(Func<UtilityEnemy, T> getMethod, Action<UtilityEnemy, T> setMethod)
            {
                this.getMethod = getMethod;
                this.setMethod = setMethod;
            }
        }
    }
}
