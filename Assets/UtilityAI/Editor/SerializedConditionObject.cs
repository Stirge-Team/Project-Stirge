using Stirge.GenericBlackboard;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
    public class SerializedConditionObject
    {
        public SerializedConditionObject()
        {
            m_constantValue = null;
            m_referenceValue = null;
            m_propertyValue = default;
            m_type = null;
            m_valueType = ConditionValueType.Constant;

            m_changed = false;
        }
        
        private object m_constantValue;
        private Object m_referenceValue;
        private BlackboardPropertyName m_propertyValue;

        private Type m_type;
        private ConditionValueType m_valueType;

        private bool m_changed;

        public bool IsNull
        {
            get
            {
                return m_valueType switch
                {
                    ConditionValueType.Constant => m_constantValue == null,
                    ConditionValueType.Reference => m_referenceValue == null,
                    ConditionValueType.Property => m_propertyValueIsNull,
                    _ => true,
                };
            }
        }
        public object constantValue
        {
            get => m_constantValue;
            set
            {
                if (m_constantValue != value)
                {
                    m_referenceValue = null;
                    m_propertyValueIsNull = true;

                    m_constantValue = value;
                    m_changed = true;
                }
            }
        }
        public Object referenceValue
        {
            get => m_referenceValue;
            set
            {
                if (m_referenceValue != value)
                {
                    m_constantValue = null;
                    m_propertyValueIsNull = true;

                    m_referenceValue = value;
                    m_changed = true;
                }
            }
        }
        public BlackboardPropertyName propertyValue
        {
            get => m_propertyValue;
            set
            {
                if (!m_propertyValue.Equals(value))
                {
                    m_constantValue = null;
                    m_referenceValue = null;

                    m_propertyValue = value;
                    m_propertyValueIsNull = false;
                    m_changed = true;
                }
            }
        }
        public Type type
        {
            get => m_type;
            set
            {
                if (m_type != value)
                {
                    m_type = value;
                    m_changed = true;
                }
            }
        }

        public ConditionValueType valueType
        {
            get => m_valueType;
            set
            {
                if (m_valueType != value)
                {
                    m_valueType = value;
                    m_type = null;
                    m_changed = true;
                }
            }
        }
        public bool changed
        {
            get => m_changed;
            set => m_changed = value;
        }
    }
}
