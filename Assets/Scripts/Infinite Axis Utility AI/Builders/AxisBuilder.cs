using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public class AxisBuilder : MonoBehaviour, IAxisBuilder
    {
        [SerializeField] private string m_name;
        [SerializeField] private Type m_axisType;
        [SerializeField] private object[] m_parameters;
        [SerializeField] private Func<float> m_getValue;
        public AxisBuilder(Type axisType, object[] parameters = null, Func<float> getValue = null)
        {
            m_axisType = axisType;
            m_parameters = parameters;
            m_getValue = getValue;
        }

        public Type axisType => m_axisType;

        public bool AreEqual(object[] parameters)
        {
            if (m_parameters == null)
            {
                return parameters == null;
            }

            if (parameters == null)
            {
                return false;
            }

            if (m_parameters.Length != parameters.Length)
            {
                return false;
            }

            for (int i = 0, count = m_parameters.Length; i < count; ++i)
            {
                if (m_parameters[i] == null)
                {
                    if (parameters[i] != null)
                    {
                        return false;
                    }
                }
                else if (!m_parameters[i].Equals(parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public Axis Build()
        {
            Axis axis = m_parameters != null
                ? Axis.Create(m_axisType, m_parameters)
                : Axis.Create(m_axisType);
            axis.name = m_name;
            return axis;
        }
    }
}
