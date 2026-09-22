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

    public abstract class GenericCondition<T1, T2> : ICondition
    {
        protected enum ConditionType
        {
            BothObject = 0,
            FirstObjectSecondProperty = 1,
            FirstPropertySecondObject = 2,
            BothProperty = 3
        }

        public static Type FirstType => typeof(T1);
        public static Type SecondType => typeof(T2);

        public static bool Equatable = (FirstType == SecondType) || Comparable;
        public static bool Comparable = StirgeTypeHelper.IsNumericType(FirstType) && StirgeTypeHelper.IsNumericType(SecondType);

        private Operation m_operation;
        private ConditionType m_type;

        #region Objects
        private T1 m_firstObject;
        private T2 m_secondObject;
        private BlackboardPropertyName m_firstPropertyName;
        private BlackboardPropertyName m_secondPropertyName;
        private bool m_firstIsForUser;
        private bool m_secondIsForUser;

        private T1 GetFirstObject(CombatEntity user, CombatEntity target)
        {
            return m_type switch
            {
                ConditionType.BothObject or ConditionType.FirstObjectSecondProperty => m_firstObject,
                ConditionType.FirstPropertySecondObject or ConditionType.BothProperty =>
                    GetT1PropertyMethod(m_firstIsForUser ? user : target, m_firstPropertyName),
                _ => default,
            };
        }
        private T2 GetSecondObject(CombatEntity user, CombatEntity target)
        {
            return m_type switch
            {
                ConditionType.BothObject or ConditionType.FirstPropertySecondObject => m_secondObject,
                ConditionType.FirstObjectSecondProperty or ConditionType.BothProperty =>
                    GetT2PropertyMethod(m_secondIsForUser ? user : target, m_secondPropertyName),
                _ => default,
            };
        }

        protected abstract T1 GetT1PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName);
        protected abstract T2 GetT2PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName);
        #endregion

        #region Init
        public void Init(Operation operation, object firstObject, object secondObject)
        {
            m_operation = operation;
            m_firstObject = (T1)firstObject;
            m_secondObject = (T2)secondObject;
            m_type = ConditionType.BothObject;
        }
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, bool propertyTargetIsUser)
        {
            m_operation = operation;
            m_firstObject = (T1)obj;
            m_secondPropertyName = propertyName;
            m_secondIsForUser = propertyTargetIsUser;
            m_type = ConditionType.FirstObjectSecondProperty;
        }
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, bool propertyTargetIsUser)
        {
            m_operation = operation;
            m_firstPropertyName = propertyName;
            m_secondObject = (T2)obj;
            m_firstIsForUser = propertyTargetIsUser;
            m_type = ConditionType.FirstPropertySecondObject;
        }
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, bool firstPropertyTargetIsUser, bool secondPropertyTargetIsUser)
        {
            m_operation = operation;
            m_firstPropertyName = firstPropertyName;
            m_secondPropertyName = secondPropertyName;
            m_firstIsForUser = firstPropertyTargetIsUser;
            m_secondIsForUser = secondPropertyTargetIsUser;
            m_type = ConditionType.BothProperty;
        }
        #endregion       

        public bool Evaluate(UtilityEnemy user, CombatEntity target)
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
                    Operation.Equal => GetFirstObject(user, target).Equals(GetSecondObject(user, target)),
                    Operation.NotEqual => !GetFirstObject(user, target).Equals(GetSecondObject(user, target)),
                    _ => LogNotComparableError(), // if operation is not Equals or NotEquals, then its trying to compare
                };
            }

            // if comparable, convert to single
            float firstValue = Convert.ToSingle(GetFirstObject(user, target));
            float secondValue = Convert.ToSingle(GetSecondObject(user, target));

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

    public class BothClassGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : class where T2 : class
    {
        protected override T1 GetT1PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetClassValue(target, propertyName, out T1 value))
                return value;
            return null;
        }
        protected override T2 GetT2PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetClassValue(target, propertyName, out T2 value))
                return value;
            return null;
        }
    }
    public class ClassStructGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : class where T2 : struct
    {
        protected override T1 GetT1PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetClassValue(target, propertyName, out T1 value))
                return value;
            return null;
        }
        protected override T2 GetT2PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetStructValue(target, propertyName, out T2 value))
                return value;
            return new T2();
        }
    }
    public class StructClassGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : struct where T2 : class
    {
        protected override T1 GetT1PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetStructValue(target, propertyName, out T1 value))
                return value;
            return new T1();
        }
        protected override T2 GetT2PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetClassValue(target, propertyName, out T2 value))
                return value;
            return null;
        }
    }
    public class BothStructGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : struct where T2 : struct
    {
        protected override T1 GetT1PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetStructValue(target, propertyName, out T1 value))
                return value;
            return new T1();
        }
        protected override T2 GetT2PropertyMethod(CombatEntity target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetStructValue(target, propertyName, out T2 value))
                return value;
            return new T2();
        }
    }
}
