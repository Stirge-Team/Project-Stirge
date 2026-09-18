using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
    using GenericBlackboard;

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
        [SerializeField] private bool m_firstPropertyTargetIsUser;
        [SerializeField] private bool m_secondPropertyTargetIsUser;
        [SerializeField] private string m_firstTypeAssemblyQualifiedName;
        [SerializeField] private string m_secondTypeAssemblyQualifiedName;

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

            Type firstType = Type.GetType(m_firstTypeAssemblyQualifiedName);
            Type secondType = Type.GetType(m_secondTypeAssemblyQualifiedName);

            Type conditionType = (firstType.IsClass, secondType.IsClass) switch
            {
                (true, true) => typeof(BothClassGenericCondition<,>),
                (true, false) => typeof(ClassStructGenericCondition<,>),
                (false, true) => typeof(StructClassGenericCondition<,>),
                (false, false) => typeof(BothStructGenericCondition<,>)
            };

            Type genericConditionType = conditionType.MakeGenericType(firstType, secondType);
            ICondition newCondition = Activator.CreateInstance(genericConditionType) as ICondition;

            switch (firstIsProperty, secondIsProperty)
            {
                case (true, true):
                    newCondition.Init(m_operation, m_firstPropertyName, m_secondPropertyName, m_firstPropertyTargetIsUser, m_secondPropertyTargetIsUser);
                    break;
                case (true, false):
                    newCondition.Init(m_operation, m_firstPropertyName, secondObject, m_firstPropertyTargetIsUser);
                    break;
                case (false, true):
                    newCondition.Init(m_operation, firstObject, m_secondPropertyName, m_secondPropertyTargetIsUser);
                    break;
                case (false, false):
                    newCondition.Init(m_operation, firstObject, secondObject);
                    break;
            }
            
            return newCondition;
        }
    }
}
