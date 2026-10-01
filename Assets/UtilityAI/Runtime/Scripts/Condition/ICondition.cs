using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using GenericBlackboard;

    public interface ICondition
    {
        public void Init(Operation operation, object firstObject, object secondObject);
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, EntityTargetType propertyTarget);
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, EntityTargetType propertyTarget);
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, EntityTargetType firstPropertyTarget, EntityTargetType secondPropertyTarget);
        public bool Evaluate(UtilityEnemy user, CombatEntity target);
    }
}
