using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    public interface IScorable
    {        
        public float Evaluate(UtilityEnemy user, CombatEntity target);
    }
}
