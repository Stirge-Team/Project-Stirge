using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stirge.UtilityAI.Demo.Axes
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;

    public class PolynomialAxis : Axis, ISetupable<int, float[], BlackboardPropertyName>
    {
        private int m_polynomialType;
        private float[] m_params;
        private BlackboardPropertyName m_floatPropertyName;
        
        void ISetupable<int, float[], BlackboardPropertyName>.Setup(int polynomialType, float[] parameters, BlackboardPropertyName floatPropertyName)
        {
            m_polynomialType = polynomialType;
            m_params = parameters;
            m_floatPropertyName = floatPropertyName;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_floatPropertyName, out float value))
            {
                // ensure the number of parameters is correct for the polynomial type
                // length of params should always be polynomialType + 1
                // e.g. quadratic polynomial (ax^2 + bx + c) has highest power of 2 (polynomialType) with 3 params (params.Length == 3)
                int paramCount = m_polynomialType + 1;
                if (paramCount != m_params.Length)
                    return 0;

                // build terms
                List<float> terms = new();
                for (int i = 0; i < paramCount; i++)
                {
                    terms[i] = m_params[i] * Mathf.Pow(value, paramCount - i);
                }

                return terms.Sum();
            }
            return 0;
        }
    }
}
