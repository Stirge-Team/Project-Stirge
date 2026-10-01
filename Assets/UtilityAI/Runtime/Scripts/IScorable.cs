using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    public interface IScorable
    {
        /// <summary>
        /// Returns the score for this <see cref="IScorable"/>.<br/>
        /// </summary>
        public float Evaluate(UtilityEnemy user, CombatEntity target);
    }
}
