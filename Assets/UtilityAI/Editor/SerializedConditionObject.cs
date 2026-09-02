using Stirge.GenericBlackboard;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
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
                    ConditionValueType.Property => m_propertyValue.IsNull,
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
                    if (value != null)
                    {
                        m_referenceValue = null;
                        m_propertyValue = default;
                    }

                    m_constantValue = value;
                    m_valueType = ConditionValueType.Constant;
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
                    if (value != null)
                    {
                        m_constantValue = null;
                        m_propertyValue = default;
                    }

                    m_referenceValue = value;
                    m_valueType = ConditionValueType.Reference;
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
                    if (!value.IsNull)
                    {
                        m_constantValue = null;
                        m_referenceValue = null;
                    }

                    m_propertyValue = value;
                    m_valueType = ConditionValueType.Property;
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

                    switch (value)
                    {
                        case ConditionValueType.Constant:
                            referenceValue = null;
                            propertyValue = default;

                            m_type = constantValue != null ? constantValue.GetType() : null;
                            break;
                        case ConditionValueType.Reference:
                            constantValue = null;
                            propertyValue = default;

                            m_type = referenceValue != null ? referenceValue.GetType() : null;
                            break;
                        case ConditionValueType.Property:
                            constantValue = null;
                            referenceValue = null;

                            m_type = !propertyValue.IsNull ? propertyValue.Type : null;
                            break;
                    }

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
