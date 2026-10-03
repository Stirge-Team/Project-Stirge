using System;
using System.Reflection;
using UnityEngine;

// https://www.youtube.com/watch?v=er9nD-usM1A&t=588s
// https://www.reddit.com/r/csharp/comments/tz0jso/why_is_reflection_faster_using_delegates/

namespace Stirge.GenericBlackboard
{
    public sealed class BlackboardTable<TBase, TValue> : IBlackboardTable<TBase>
    {
        private BlackboardTableProperty[] m_values;

        public Type valueType => typeof(TValue);
        public int count => m_values.Length;

        public void Setup(PropertyInfo[] propertyInfos)
        {
            int count = propertyInfos.Length;
            m_values = new BlackboardTableProperty[count];
            for (int i = 0; i < count; i++)
            {
                PropertyInfo info = propertyInfos[i];
                MethodInfo getMethod = info.GetGetMethod();
                MethodInfo setMethod = info.GetSetMethod();
                m_values[i] = new BlackboardTableProperty(
                    info.Name,
                    getMethod == null ? null : (Func<TBase, TValue>)Delegate.CreateDelegate(typeof(Func<TBase, TValue>), info.GetGetMethod()),
                    setMethod == null ? null : (Action<TBase, TValue>)Delegate.CreateDelegate(typeof(Action<TBase, TValue>), info.GetSetMethod())
                );
            }
        }
        
        public TValue GetValue(TBase target, int index)
        {
            return m_values[index].getMethod(target);
        }
        public object GetObjectValue(TBase target, int index)
        {
            return GetValue(target, index);
        }

        public void SetValue(TBase target, TValue value, int index)
        {
            BlackboardTableProperty property = m_values[index];
            if (property.setMethod != null)
                property.setMethod(target, value);
            else
                Debug.LogError($"Property with Name '{property.propertyName}' is read-only and/or does not have a defined Set method!");
        }

        public void SetObjectValue(TBase target, object value, int index)
        {
            SetValue(target, (TValue)value, index);
        }

        private readonly struct BlackboardTableProperty
        {
            public readonly string propertyName;
            public readonly Func<TBase, TValue> getMethod;
            public readonly Action<TBase, TValue> setMethod;

            public BlackboardTableProperty(string name, Func<TBase, TValue> getMethod, Action<TBase, TValue> setMethod)
            {
                propertyName = name;
                this.getMethod = getMethod;
                this.setMethod = setMethod;
            }
        }
    }
}
