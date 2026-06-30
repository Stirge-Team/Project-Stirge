using UnityEngine;
using System;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI.Callbacks
{
    public abstract class CallbackBinding_Base : ScriptableObject
    {
        [SerializeField] private string m_key;
        private BlackboardPropertyName m_propertyName;

        public BlackboardPropertyName propertyName
        {
            get
            {
                if (m_propertyName == default)
                {
                    m_propertyName = new(m_key);
                }
                return m_propertyName;
            }
        }

        public abstract Type valueType { get; }

        public abstract object GetObjectValue();
    }
}
