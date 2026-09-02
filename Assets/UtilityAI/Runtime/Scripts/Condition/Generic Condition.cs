using Stirge.GenericBlackboard;
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

    public class Condition<T1, T2> : ICondition where T1 : IEquatable<T2> where T2 : IEquatable<T1>
    {
        private enum ConditionType
        {
            BothObject = 0,
            HalfNHalf = 1, // in a half n half, the first object is always the object, and the second object is always the property
            BothProperty = 2
        }

        public static Type FirstType => typeof(T1);
        public static Type SecondType => typeof(T2);

        public static bool Equatable = FirstType == SecondType || Comparable;
        public static bool Comparable = StirgeTypeHelper.IsNumericType(FirstType) && StirgeTypeHelper.IsNumericType(SecondType);

        private Action m_action;

        private ConditionType m_type;

        private Operation m_operation;
        private T1 m_firstObject;
        private T2 m_secondObject;
        private BlackboardPropertyName m_firstPropertyName;
        private BlackboardPropertyName m_secondPropertyName;

        public void Init(Operation operation, object firstObject, object secondObject, Type firstType, Type secondType)
        {
            m_operation = operation;
            m_firstObject = (T1)firstObject;
            m_secondObject = (T2)secondObject;
            m_type = ConditionType.BothObject;
        }
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, Type objType)
        {
            m_operation = operation;
        }
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName)
        {
            throw new NotImplementedException();
        }

        public void Setup(Action action)
        {
            m_action = action;
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

        object ICondition.GetFirstObject()
        {
            throw new NotImplementedException();
        }

        object ICondition.GetSecondObject()
        {
            throw new NotImplementedException();
        }

        bool ICondition.TryGetFirst<T>(out T value)
        {
            throw new NotImplementedException();
        }

        bool ICondition.TryGetSecond<T>(out T value)
        {
            throw new NotImplementedException();
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
