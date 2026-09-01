using System;
using UnityEngine;

namespace Stirge.GenericBlackboard
{
    [Serializable]
    public struct BlackboardPropertyName : IEquatable<BlackboardPropertyName>
    {
        [SerializeField] private string m_propertyName;
        [SerializeField] private int m_hash;
        [SerializeField] private Type m_type;

        public string Name => m_propertyName;
        public int Hash => m_hash;
        public Type Type => m_type;

        public bool IsNull => m_type == null;

        public BlackboardPropertyName(string name, Type type)
        {
            m_propertyName = name;
            m_hash = GetHashCode(name);
            m_type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardPropertyName other && Equals(other);
        }
        public bool Equals(BlackboardPropertyName other)
        {
            return other.Hash == Hash;
        }
        public override readonly string ToString()
        {
            return m_propertyName;
        }
        public override int GetHashCode()
        {
            return Hash;
        }

        public static int GetHashCode(string name)
        {
            return name.GetHashCode(0);
        }
    }
}
