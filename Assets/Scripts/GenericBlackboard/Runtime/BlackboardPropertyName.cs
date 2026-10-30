using System;
using UnityEngine;

namespace Stirge.GenericBlackboard
{
    [Serializable]
    public struct BlackboardPropertyName : IEquatable<BlackboardPropertyName>
    {
        [SerializeField] private string m_propertyName;
        [SerializeField] private int m_hash;

        public readonly string Name => m_propertyName;
        public readonly int Hash => m_hash;

        public readonly bool IsNull => string.IsNullOrEmpty(m_propertyName);

        public BlackboardPropertyName(string name)
        {
            m_propertyName = name;
            m_hash = GetHashCode(name);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is BlackboardPropertyName other && Equals(other);
        }
        public readonly bool Equals(BlackboardPropertyName other)
        {
            return other.m_hash == m_hash;
        }
        public override readonly string ToString()
        {
            return m_propertyName;
        }
        public override readonly int GetHashCode()
        {
            return m_hash;
        }

        public static int GetHashCode(string name)
        {
            return name.GetHashCode(0);
        }
    }
}
