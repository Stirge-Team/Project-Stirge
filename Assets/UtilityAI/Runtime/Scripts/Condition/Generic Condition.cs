using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using GenericBlackboard;
    using Tools;

    public enum Operation
    {
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
    }

    public class Condition<T1, T2> : ICondition // where T1 : IEquatable<T2> where T2 : IEquatable<T1>
    {
        private enum ConditionType
        {
            BothObject = 0,
            HalfFirstObj = 1,
            HalfSecondObj = 2,
            BothProperty = 3
        }

        public static Type FirstType => typeof(T1);
        public static Type SecondType => typeof(T2);

        public static bool Equatable = (FirstType == SecondType) || Comparable;
        public static bool Comparable = StirgeTypeHelper.IsNumericType(FirstType) && StirgeTypeHelper.IsNumericType(SecondType);

        private Action m_action;

        private ConditionType m_type;

        private Operation m_operation;
        private T1 m_firstObject;
        private T2 m_secondObject;
        private BlackboardPropertyName m_firstPropertyName;
        private BlackboardPropertyName m_secondPropertyName;
        private bool m_firstIsStruct;
        private bool m_secondIsSruct;

        private T1 FirstObject
        {
            get
            {
                switch (m_type)
                {
                    case ConditionType.BothObject:
                    case ConditionType.HalfFirstObj:
                        return m_firstObject;
                    case ConditionType.HalfSecondObj:
                    case ConditionType.BothProperty:
                        GenericBlackboard<CombatEntity>.TryGetObjectValue(m_action.Target, FirstType, m_firstPropertyName, out var value);
                        return (T1)value;
                    default:
                        return default;
                }
            }
        }
        private T2 SecondObject
        {
            get
            {
                switch (m_type)
                {
                    case ConditionType.BothObject:
                    case ConditionType.HalfSecondObj:
                        return m_secondObject;
                    case ConditionType.HalfFirstObj:
                    case ConditionType.BothProperty:
                        if (m_secondIsSruct)
                            GenericBlackboard<CombatEntity>.TryGetStructValue(m_action.Target, m_secondPropertyName, out T2 value);
                        else
                            GenericBlackboard<CombatEntity>.TryGetObjectValue(m_action.Target, SecondType, m_secondPropertyName, out var value);
                        return value;
                    default:
                        return default;
                }
            }
        }

        #region Init
        public void Init(Operation operation, object firstObject, object secondObject, bool firstIsStruct, bool secondIsStruct)
        {
            m_operation = operation;
            m_firstObject = (T1)firstObject;
            m_secondObject = (T2)secondObject;
            m_type = ConditionType.BothObject;
            m_firstIsStruct = firstIsStruct;
            m_secondIsSruct = secondIsStruct;
        }
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, bool firstIsStruct, bool secondIsStruct)
        {
            m_operation = operation;
            m_firstObject = (T1)obj;
            m_secondPropertyName = propertyName;
            m_type = ConditionType.HalfFirstObj;
            m_firstIsStruct = firstIsStruct;
            m_secondIsSruct = secondIsStruct;
        }
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, bool firstIsStruct, bool secondIsStruct)
        {
            m_operation = operation;
            m_firstPropertyName = propertyName;
            m_secondObject = (T2)obj;
            m_type = ConditionType.HalfSecondObj;
            m_firstIsStruct = firstIsStruct;
            m_secondIsSruct = secondIsStruct;
        }
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, bool firstIsStruct, bool secondIsStruct)
        {
            m_operation = operation;
            m_firstPropertyName = firstPropertyName;
            m_secondPropertyName = secondPropertyName;
            m_type = ConditionType.BothProperty;
            m_firstIsStruct = firstIsStruct;
            m_secondIsSruct = secondIsStruct;
        }
        #endregion

        public void Setup(Action action)
        {
            m_action = action;
        }

        public bool Evaluate(CombatEntity user, CombatEntity target)
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
                    _ => LogNotComparableError(), // if operation is not Equals or NotEquals, then its trying to compare
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
