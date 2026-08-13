using System;
using UnityEngine;

namespace Stirge.InfiniteAxis.Blackboard
{
    [Serializable]
    public struct BlackboardPropertyName
    {
        [SerializeField] private string m_propertyName;
        [SerializeField] private int m_hash;

        public string Name => m_propertyName;
        public int Hash => m_hash;

        public BlackboardPropertyName(string name)
        {
            m_propertyName = name;
            m_hash = GetHashCode(name);
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardPropertyName other && Equals(other);
        }
        public bool Equals(BlackboardPropertyName other)
        {
            return other.Hash == Hash;
        }
        public override string ToString()
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
