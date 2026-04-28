using UnityEngine;

namespace Stirge.Combat
{
    using Stirge.AI;

    [CreateAssetMenu(fileName = "New Enemy Attack data", menuName = "Stirge/Enemy Attack Data", order = 1)]
    public class EnemyAttackData : ScriptableObject
    {
        // Could use a Behaviour tree to create Attacking logic and create custom functions in Enemy and Agent for unique behaviours
        // such as moving to a target location or jumping.
        // at this point is it easier to just create custom Behaviours for each attack?
        // discussion required methinks

        private float m_totalTime;

        public void DetermineAttack()
        {

        }
    }
}
