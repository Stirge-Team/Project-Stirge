using Stirge.GenericBlackboard;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
    public enum ConditionValueType
    {
        Constant = 0,
        Reference = 1,
        Property = 2
    }

    [CreateAssetMenu(menuName = "Utility AI/Serialized Condition", fileName = "New Condition", order = 450)]
    public class SerializedCondition : ScriptableObject
    {
        [SerializeField] private Operation m_operation;
        [SerializeReference] private object m_firstConstantObject;
        [SerializeReference] private object m_secondConstantObject;
        [SerializeField] private Object m_firstReferenceObject;
        [SerializeField] private Object m_secondReferenceObject;
        [SerializeField] private BlackboardPropertyName m_firstPropertyName;
        [SerializeField] private BlackboardPropertyName m_secondPropertyName;

        [SerializeField] private bool m_isValid;

        public ICondition CreateRuntimeCondition()
        {
            if (!m_isValid)
            {
                Debug.LogError("This SerializedCondition is not valid! Click me to find out who :3", this);
                return null;
            }

            object firstObject = null;
            BlackboardPropertyName firstPropertyName = default;
            ConditionValueType firstValueType;
            if (m_firstConstantObject != null)
            {
                firstObject = m_firstConstantObject;
                firstValueType = ConditionValueType.Constant;
            }
            else if (m_firstReferenceObject != null)
            {
                firstObject = m_firstReferenceObject;
                firstValueType = ConditionValueType.Reference;
            }
            else
            {
                firstPropertyName = m_firstPropertyName;
                firstValueType = ConditionValueType.Property;
            }

            object secondObject = null;
            BlackboardPropertyName secondPropertyName = default;
            ConditionValueType secondValueType;
            if (m_secondConstantObject != null)
            {
                secondObject = m_secondConstantObject;
                secondValueType = ConditionValueType.Constant;
            }
            else if (m_secondReferenceObject != null)
            {
                secondObject = m_secondReferenceObject;
                secondValueType = ConditionValueType.Reference;
            }
            else
            {
                secondPropertyName = m_secondPropertyName;
                secondValueType = ConditionValueType.Property;
            }

            Type firstType = firstValueType switch
            {
                ConditionValueType.Constant | ConditionValueType.Reference => firstObject.GetType(),
                ConditionValueType.Property => firstPropertyName.Type,
                _ => null
            };

            Type secondType = secondValueType switch
            {
                ConditionValueType.Constant | ConditionValueType.Reference => secondObject.GetType(),
                ConditionValueType.Property => secondPropertyName.Type,
                _ => null
            };

            if (useGenericCondition)
            {
                Type genericConditionType = typeof(GenericCondition<,>).MakeGenericType(firstType, secondType);
                ICondition newCondition = Activator.CreateInstance(genericConditionType) as ICondition;
                newCondition.Init(m_operation, firstObject, secondObject, firstType, secondType);
                return newCondition;
            }
            else
            {
                ICondition newCondition = Condition.Create(m_operation, firstObject, secondObject, firstType, secondType);
                return newCondition;
            }
        }
    }
}
