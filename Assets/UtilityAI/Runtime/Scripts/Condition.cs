using Stirge.Combat;
using Stirge.Tools;
using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public enum Operation
    {
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
    }

    public class Condition
    {
        private Operation m_operation;
        private object m_firstObject;
        private object m_secondObject;

        private Type m_firstType;
        private Type m_secondType;

        private bool m_firstIsNumeric;
        private bool m_secondIsNumeric;

        public bool Evaluate()
        {
            if (m_firstIsNumeric && m_secondIsNumeric)
            {
                float firstNumber = Convert.ToSingle(m_firstObject);
                float secondNumber = Convert.ToSingle(m_secondObject);

                return m_operation switch
                {
                    Operation.Equal => firstNumber == secondNumber || Mathf.Approximately(firstNumber, secondNumber),
                    Operation.NotEqual => firstNumber != secondNumber,
                    Operation.LessThan => firstNumber < secondNumber,
                    Operation.GreaterThan => firstNumber > secondNumber,
                    Operation.LessThanOrEqual => firstNumber <= secondNumber || Mathf.Approximately(firstNumber, secondNumber),
                    Operation.GreaterThanOrEqual => firstNumber >= secondNumber || Mathf.Approximately(firstNumber, secondNumber),
                    _ => false,
                };
            }
            else if (m_firstType == m_secondType)
            {
                return m_operation switch
                {
                    Operation.Equal => m_firstObject.Equals(m_secondObject),
                    Operation.NotEqual => !m_firstObject.Equals(m_secondObject),
                    _ => false,
                };
            }

            Debug.LogError($"Condition is invalid as types of {m_firstObject} and {m_secondObject} cannot be compared!");
            return false;
        }

        public static Condition Create(Operation operation, object firstObject, object secondObject)
        {
            Condition condition = new()
            {
                m_operation = operation,
                m_firstObject = firstObject,
                m_secondObject = secondObject,

                m_firstType = firstObject.GetType(),
                m_secondType = secondObject.GetType()
            };

            condition.m_firstIsNumeric = StirgeTypeHelper.IsNumericType(condition.m_firstType);
            condition.m_secondIsNumeric = StirgeTypeHelper.IsNumericType(condition.m_secondType);

            return condition;
        }
    }
}
