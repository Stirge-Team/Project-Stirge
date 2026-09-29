using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;

    public interface IScoringMethod<in T> where T : IScorable
    {
        public float Evaluate(UtilityEnemy user, CombatEntity target);
    }
}
