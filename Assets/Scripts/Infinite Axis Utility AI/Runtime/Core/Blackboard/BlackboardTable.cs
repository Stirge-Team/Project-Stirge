using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public sealed class BlackboardTable<T> : IBlackboardTable
    {
        private readonly List<T> m_values = new();
        private readonly Stack<int> m_freeIndices = new();

        public Type valueType => typeof(T);

        public int count => m_values.Count - m_freeIndices.Count;

        public T GetValue(int index)
        {
            return m_values[index];
        }

        public object GetObjectValue(int index)
        {
            return GetValue(index);
        }

        public void SetValue(T value, int index)
        {
            m_values[index] = value;
        }

        public void SetObjectValue(object value, int index)
        {
            SetValue((T)value, index);
        }

        public int AddValue(T value)
        {
            if (m_freeIndices.TryPop(out int index))
            {
                SetValue(value, index);
                return index;
            }

            m_values.Add(value);
            return m_values.Count - 1;
        }

        public int AddObjectValue(object value)
        {
            return AddValue((T)value);
        }

        public void RemoveAt(int index)
        {
            m_values[index] = default;
            m_freeIndices.Push(index);
        }

        public void Clear()
        {
            for (int i = 0, valueCount = m_values.Count; i < valueCount; i++)
            {
                if (!m_freeIndices.Contains(i))
                {
                    RemoveAt(i);
                }
            }
        }
    }
}
