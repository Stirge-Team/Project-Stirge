using Stirge.Tools;
using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public class GenericCondition<T1, T2> : ICondition
    {
        public static Type FirstType => typeof(T1);
        public static Type SecondType => typeof(T2);

        public static bool Equatable = FirstType == SecondType || Comparable;
        public static bool Comparable = StirgeTypeHelper.IsNumericType(FirstType) && StirgeTypeHelper.IsNumericType(SecondType);

        private Operation m_operation;
        private T1 m_firstObject;
        private T2 m_secondObject;

        public void Init(Operation operation, object firstObject, object secondObject)
        {
            m_operation = operation;
            m_firstObject = (T1)firstObject;
            m_secondObject = (T2)secondObject;
        }

        public bool Evaluate()
        {
            // if not comparable
            if (!Comparable)
            {
                // if not equatable
                if (!Equatable)
                {
                    LogNotEquatableError();
                    return false;
                }

                return m_operation switch
                {
                    Operation.Equal => m_firstObject.Equals(m_secondObject),
                    Operation.NotEqual => !m_firstObject.Equals(m_secondObject),
                    _ => LogNotComparableError(),
                };
            }

            // if comparable, convert to single
            float firstValue = Convert.ToSingle(m_firstObject);
            float secondValue = Convert.ToSingle(m_secondObject);

            return m_operation switch
            {
                Operation.Equal => firstValue == secondValue || Mathf.Approximately(firstValue, secondValue),
                Operation.NotEqual => firstValue != secondValue,
                Operation.LessThan => firstValue < secondValue,
                Operation.GreaterThan => firstValue > secondValue,
                Operation.LessThanOrEqual => firstValue <= secondValue || Mathf.Approximately(firstValue, secondValue),
                Operation.GreaterThanOrEqual => firstValue >= secondValue || Mathf.Approximately(firstValue, secondValue),
                _ => false,
            };
        }

        private static void LogNotEquatableError()
        {
            Debug.LogError($"Types {FirstType.Name} and {SecondType.Name} are not equatable!");
        }
        private static bool LogNotComparableError()
        {
            Debug.LogError($"Types {FirstType.Name} and {SecondType.Name} are not comparable!");
            return false;
        }
    }
}
