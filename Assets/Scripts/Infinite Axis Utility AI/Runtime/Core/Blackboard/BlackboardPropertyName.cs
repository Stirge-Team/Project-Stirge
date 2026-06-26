using System;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public readonly struct BlackboardPropertyName : IEquatable<BlackboardPropertyName>
    {
        private static readonly System.Collections.Generic.Dictionary<int, string> s_names = new(1000);

        public readonly int id;

        public BlackboardPropertyName(string name)
        {
            id = name.GetHashCode();
            s_names[id] = name;
        }

        public string name
        {
            get
            {
                if (s_names.TryGetValue(id, out string propertyName))
                    return propertyName;
                else
                    return string.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardPropertyName other && other.id == id;
        }
        public bool Equals(BlackboardPropertyName other)
        {
            return other.id == id;
        }

        public override int GetHashCode()
        {
            return id;
        }
        public override string ToString()
        {
            return $"{id.ToString()}({name})";
        }

        public static bool operator ==(BlackboardPropertyName lhs, BlackboardPropertyName rhs)
        {
            return lhs.id == rhs.id;
        }
        public static bool operator !=(BlackboardPropertyName lhs, BlackboardPropertyName rhs)
        {
            return lhs.id != rhs.id;
        }
    }
}
