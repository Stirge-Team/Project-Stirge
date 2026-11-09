using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    /// <summary>
    /// Do not implement this interface in your own Conditions.
    /// </summary>
    public interface ICondition
    {
        public string DisplayName { get; }
        
        public bool Evaluate(CombatEntity user, CombatEntity target);

        public void Setup<T>(T scorable, string displayName) where T : class, IScorable;
    }
}
