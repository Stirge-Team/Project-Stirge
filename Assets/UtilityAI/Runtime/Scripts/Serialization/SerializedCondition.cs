using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu]
    public class SerializedCondition : ScriptableObject
    {
        [SerializeField] private Operation m_operation;
        [SerializeReference] private object m_firstConstantObject;
        [SerializeReference] private object m_secondConstantObject;
        [SerializeField] private Object m_firstReferenceObject;
        [SerializeField] private Object m_secondReferenceObject;

        [SerializeField] private bool m_isValid;

        public ICondition CreateRuntimeCondition()
        {
            if (!m_isValid)
            {
                Debug.LogError("This SerializedCondition is not valid! Click me to find out who :3", this);
                return null;
            }

            bool firstIsConstant = m_firstConstantObject != null;
            bool secondIsConstant = m_secondConstantObject != null;

            Type firstType = firstIsConstant ? m_firstConstantObject.GetType() : m_firstReferenceObject.GetType();
            Type secondType = secondIsConstant ? m_secondConstantObject.GetType() : m_secondReferenceObject.GetType();

            Type genericConditionType = typeof(GenericCondition<,>).MakeGenericType(firstType, secondType);

            ICondition newCondition = Activator.CreateInstance(genericConditionType) as ICondition;
            newCondition.Init(m_operation, firstIsConstant ? m_firstConstantObject : m_firstReferenceObject,
                                           secondIsConstant ? m_secondConstantObject : m_secondReferenceObject);
            return newCondition;
        }
    }
}
