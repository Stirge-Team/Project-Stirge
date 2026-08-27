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
            m_constantValue = null;
            m_type = null;
            m_isConstantValue = false;

            m_changed = false;
        }
        
        private object m_constantValue;
        private Object m_referenceValue;
        private Type m_type;
        private bool m_isConstantValue;

        private bool m_changed;

        public bool IsNull => m_isConstantValue ? m_constantValue == null : m_referenceValue == null;
        public object constantValue
        {
            get => m_constantValue;
            set
            {
                if (m_constantValue != value)
                {
                    m_referenceValue = null;
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
                    m_referenceValue = value;
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

        public bool isConstantValue
        {
            get => m_isConstantValue;
            set
            {
                if (m_isConstantValue != value)
                {
                    m_isConstantValue = value;
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
