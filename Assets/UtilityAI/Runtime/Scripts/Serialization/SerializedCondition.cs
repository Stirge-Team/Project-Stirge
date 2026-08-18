using Stirge.Tools;
using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu]
    public class SerializedCondition : ScriptableObject
    {
        [SerializeField] private Operation m_operation;
        [SerializeReference] private object m_firstObject;
        [SerializeReference] private object m_secondObject;

        public ICondition CreateRuntimeCondition()
        {
            Type firstType = m_firstObject.GetType();
            Type secondType = m_secondObject.GetType();

            Type genericConditionType = typeof(GenericCondition<,>).MakeGenericType(firstType, secondType);

            ICondition newCondition = Activator.CreateInstance(genericConditionType) as ICondition;
            newCondition.Init(m_operation, m_firstObject, m_secondObject);
            return newCondition;
        }
    }
}
