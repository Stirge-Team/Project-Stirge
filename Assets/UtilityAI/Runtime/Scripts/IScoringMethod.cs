using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    /// <summary>
    /// Do not implement this interface in your own ScoringMethods.
    /// </summary>
    public interface IScoringMethod
    {
        public abstract float Evaluate(UtilityEnemy user, CombatEntity target);
        public abstract void Setup<T>(T scorable, float scoreScaling) where T : class, IScorable;
    }
}
