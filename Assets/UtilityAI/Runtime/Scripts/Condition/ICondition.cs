using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using GenericBlackboard;

    public interface ICondition
    {
        public void Init(Operation operation, object firstObject, object secondObject);
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, bool propertyTargetIsUser);
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, bool propertyTargetIsUser);
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, bool firstPropertyTargetIsUser, bool secondPropertyTargetIsUser);
        public void Setup(Action action);
        public bool Evaluate(CombatEntity user, CombatEntity target);
    }
}
