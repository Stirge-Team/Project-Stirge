using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using GenericBlackboard;

    public interface ICondition
    {
        public void Setup<T>(T scorable, Operation operation, object firstObject, object secondObject) where T : class, IScorable;
        public void Setup<T>(T scorable, Operation operation, object obj, BlackboardPropertyName propertyName, ConditionPropertyTarget propertyTarget) where T : class, IScorable;
        public void Setup<T>(T scorable, Operation operation, BlackboardPropertyName propertyName, object obj, ConditionPropertyTarget propertyTarget) where T : class, IScorable;
        public void Setup<T>(T scorable, Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, ConditionPropertyTarget firstPropertyTarget, ConditionPropertyTarget secondPropertyTarget) where T : class, IScorable;
        public bool Evaluate(UtilityEnemy user, CombatEntity target);
    }
}
