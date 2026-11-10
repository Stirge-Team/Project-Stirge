using UnityEngine;

namespace Stirge.UtilityAI.Conditions
{
    using Combat;
    using Serialization;

    public class TargetWithinRangeCondition : Condition<Action>, INotSetupable
    {
        public override bool Evaluate(CombatEntity user, CombatEntity target)
        {
            if (user is UtilityEnemy enemy && enemy.HasTarget)
            {
                return Vector3.Distance(enemy.Motor.GetPosition(), enemy.TargetPosition) <= m_scorable.Range;
            }
            return false;
        }
    }
}
