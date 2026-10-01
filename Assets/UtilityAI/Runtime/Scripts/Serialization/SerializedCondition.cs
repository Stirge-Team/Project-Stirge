using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
    using GenericBlackboard;

    public enum SerializedScorableType
    {
        Action,
        MovementGoal,
        Status,
    }

    public enum ConditionValueType
    {
        Constant = 0,
        Reference = 1,
        Property = 2,
    }

    public enum ConditionPropertyTarget
    {
        User,
        Target,
        Scorable
    }

    [CreateAssetMenu(menuName = "Utility AI/Serialized Condition", fileName = "New Condition", order = 450)]
    public class SerializedCondition : ScriptableObject
    {
        [SerializeField] private SerializedScorableType m_scorableType;
        [SerializeField] private Operation m_operation;
        [SerializeField] private ConditionValueType m_firstValueType;
        [SerializeField] private ConditionValueType m_secondValueType;
        [SerializeReference] private object m_firstConstantObject;
        [SerializeReference] private object m_secondConstantObject;
        [SerializeField] private Object m_firstReferenceObject;
        [SerializeField] private Object m_secondReferenceObject;
        [SerializeField] private BlackboardPropertyName m_firstPropertyName;
        [SerializeField] private BlackboardPropertyName m_secondPropertyName;
        [SerializeField] private ConditionPropertyTarget m_firstPropertyTarget;
        [SerializeField] private ConditionPropertyTarget m_secondPropertyTarget;
        [SerializeField] private string m_firstTypeAssemblyQualifiedName;
        [SerializeField] private string m_secondTypeAssemblyQualifiedName;

        [SerializeField] private bool m_isValid;

        public void SetScorableType(SerializedScorableType scorableType)
        {
            m_scorableType = scorableType;
        }

        public ICondition CreateRuntimeCondition<TScorable>(TScorable scorable) where TScorable : class, IScorable
        {
            if (!m_isValid)
            {
                Debug.LogError("This SerializedCondition is not valid! Click me to find out who :3", this);
                return null;
            }

            object firstObject = null;
            bool firstIsProperty = false;
            switch (m_firstValueType)
            {
                case ConditionValueType.Constant:
                    firstObject = m_firstConstantObject;
                    break;
                case ConditionValueType.Reference:
                    firstObject = m_firstReferenceObject;
                    break;
                case ConditionValueType.Property:
                    firstIsProperty = true;
                    break;
            }

            object secondObject = null;
            bool secondIsProperty = false;
            switch (m_secondValueType)
            {
                case ConditionValueType.Constant:
                    secondObject = m_secondConstantObject;
                    break;
                case ConditionValueType.Reference:
                    secondObject = m_secondReferenceObject;
                    break;
                case ConditionValueType.Property:
                    secondIsProperty = true;
                    break;
            }

            Type firstType = Type.GetType(m_firstTypeAssemblyQualifiedName);
            Type secondType = Type.GetType(m_secondTypeAssemblyQualifiedName);

            // Can I improve this section? No reflection to create instance of Condition class?
            Type conditionType = (firstType.IsClass, secondType.IsClass) switch
            {
                (true, true) => typeof(BothClassGenericCondition<,,>),
                (true, false) => typeof(ClassStructGenericCondition<,,>),
                (false, true) => typeof(StructClassGenericCondition<,,>),
                (false, false) => typeof(BothStructGenericCondition<,,>)
            };

            Type genericConditionType = conditionType.MakeGenericType(typeof(TScorable), firstType, secondType);
            ICondition newCondition = Activator.CreateInstance(genericConditionType) as ICondition;
            // end section

            switch (firstIsProperty, secondIsProperty)
            {
                case (true, true):
                    newCondition.Setup(scorable, m_operation, m_firstPropertyName, m_secondPropertyName, m_firstPropertyTarget, m_secondPropertyTarget);
                    break;
                case (true, false):
                    newCondition.Setup(scorable, m_operation, m_firstPropertyName, secondObject, m_firstPropertyTarget);
                    break;
                case (false, true):
                    newCondition.Setup(scorable, m_operation, firstObject, m_secondPropertyName, m_secondPropertyTarget);
                    break;
                case (false, false):
                    newCondition.Setup(scorable, m_operation, firstObject, secondObject);
                    break;
            }
            
            return newCondition;
        }
    }
}
