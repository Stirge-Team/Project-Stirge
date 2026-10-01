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
        private EntityTargetType m_firstPropertyTarget;
        private EntityTargetType m_secondPropertyTarget;

        private T1 GetFirstObject(UtilityEnemy user, CombatEntity target)
        {
            return m_type switch
            {
                ConditionType.BothObject or ConditionType.FirstObjectSecondProperty => m_firstObject,
                ConditionType.FirstPropertySecondObject or ConditionType.BothProperty =>
                    m_firstPropertyTarget switch
                    {
                        EntityTargetType.User => GetFirstProperty(user, m_firstPropertyName),
                        EntityTargetType.Target => GetFirstProperty(target, m_firstPropertyName),
                        _ => default
                    },
                _ => default,
            };
        }
        private T2 GetSecondObject(UtilityEnemy user, CombatEntity target)
        {
            return m_type switch
            {
                ConditionType.BothObject or ConditionType.FirstPropertySecondObject => m_secondObject,
                ConditionType.FirstObjectSecondProperty or ConditionType.BothProperty =>
                    m_secondPropertyTarget switch
                    {
                        EntityTargetType.User => GetSecondProperty(user, m_secondPropertyName),
                        EntityTargetType.Target => GetSecondProperty(target, m_secondPropertyName),
                        _ => default
                    },
                _ => default,
            };
        }

        protected abstract T1 GetFirstProperty<T>(T target, BlackboardPropertyName propertyName) where T : CombatEntity;
        protected abstract T2 GetSecondProperty<T>(T target, BlackboardPropertyName propertyName) where T : CombatEntity;
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

        #region Init
        public void Init(Operation operation, object firstObject, object secondObject)
        {
            m_operation = operation;
            m_firstObject = (T1)firstObject;
            m_secondObject = (T2)secondObject;
            m_type = ConditionType.BothObject;
        }
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, EntityTargetType propertyTarget)
        {
            m_operation = operation;
            m_firstObject = (T1)obj;
            m_secondPropertyName = propertyName;
            m_secondPropertyTarget = propertyTarget;
            m_type = ConditionType.FirstObjectSecondProperty;
        }
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, EntityTargetType propertyTarget)
        {
            m_operation = operation;
            m_firstPropertyName = propertyName;
            m_secondObject = (T2)obj;
            m_firstPropertyTarget = propertyTarget;
            m_type = ConditionType.FirstPropertySecondObject;
        }
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, EntityTargetType firstPropertyTarget, EntityTargetType secondPropertyTarget)
        {
            m_operation = operation;
            m_firstPropertyName = firstPropertyName;
            m_secondPropertyName = secondPropertyName;
            m_firstPropertyTarget = firstPropertyTarget;
            m_secondPropertyTarget = secondPropertyTarget;
            m_type = ConditionType.BothProperty;
        }
        #endregion 
    }

    // These four classes exist because to get the value from the Blackboard you need to implicity provide whether the type is a class or struct
    public class BothClassGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : class where T2 : class
    {
        protected override T1 GetFirstProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetClassValue(target, propertyName, out T1 value))
                return value;
            return null;
        }
        protected override T2 GetSecondProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetClassValue(target, propertyName, out T2 value))
                return value;
            return null;
        }
    }
    public class ClassStructGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : class where T2 : struct
    {
        protected override T1 GetFirstProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetClassValue(target, propertyName, out T1 value))
                return value;
            return null;
        }
        protected override T2 GetSecondProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetStructValue(target, propertyName, out T2 value))
                return value;
            return new T2();
        }
    }
    public class StructClassGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : struct where T2 : class
    {
        protected override T1 GetFirstProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetStructValue(target, propertyName, out T1 value))
                return value;
            return new T1();
        }
        protected override T2 GetSecondProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetClassValue(target, propertyName, out T2 value))
                return value;
            return null;
        }
    }
    public class BothStructGenericCondition<T1, T2> : GenericCondition<T1, T2> where T1 : struct where T2 : struct
    {
        protected override T1 GetFirstProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<T>.TryGetStructValue(target, propertyName, out T1 value))
                return value;
            return new T1();
        }
        protected override T2 GetSecondProperty<T>(T target, BlackboardPropertyName propertyName)
        {
            if (GenericBlackboard<CombatEntity>.TryGetStructValue(target, propertyName, out T2 value))
                return value;
            return new T2();
        }
    }
}
