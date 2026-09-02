using Stirge.GenericBlackboard;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
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
            bool firstIsProperty;
            if (m_firstConstantObject != null)
            {
                firstObject = m_firstConstantObject;
                firstIsProperty = false;
            }
            else if (m_firstReferenceObject != null)
            {
                firstObject = m_firstReferenceObject;
                firstIsProperty = false;
            }
            else
            {
                firstIsProperty = true;
            }

            object secondObject = null;
            bool secondIsProperty;
            if (m_secondConstantObject != null)
            {
                secondObject = m_secondConstantObject;
                secondIsProperty = false;
            }
            else if (m_secondReferenceObject != null)
            {
                secondObject = m_secondReferenceObject;
                secondIsProperty = false;
            }
            else
            {
                secondIsProperty = true;
            }

            Type firstType = firstIsProperty ? m_firstPropertyName.Type : firstObject.GetType();
            Type secondType = secondIsProperty ? m_secondPropertyName.Type : secondObject.GetType();

            Type genericConditionType = typeof(Condition<,>).MakeGenericType(firstType, secondType);
            ICondition newCondition = Activator.CreateInstance(genericConditionType) as ICondition;

            // If both are same
            if (firstIsProperty == secondIsProperty)
            {
                // if both property types
                if (firstIsProperty)
                {
                    newCondition.Init(m_operation, m_firstPropertyName, m_secondPropertyName);
                }
                // if both object types
                else
                {
                    newCondition.Init(m_operation, firstObject, secondObject, firstType, secondType);
                }
            }
            // if one property and one object
            else
            {
                // if first is property
                if (firstIsProperty)
                {
                    newCondition.Init(m_operation, secondObject, m_firstPropertyName, secondType);
                }
                // if second is property
                else
                {
                    newCondition.Init(m_operation, firstObject, m_secondPropertyName, firstType);
                }
            }
            
            return newCondition;
        }
    }
}
