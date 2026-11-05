using UnityEngine;

namespace Stirge.UtilityAI.Actions
{
    using Combat;
    using Serialization;
    
    public class FindTargetAction : Action, ISetupable<LayerMask>
    {
        private LayerMask m_mask;

        private readonly Collider[] m_targetColliders = new Collider[5];

        public void Setup(LayerMask mask)
        {
            m_mask = mask;
        }

        public override void Perform(CombatEntity user, CombatEntity target)
        {
            if (user is UtilityEnemy enemy)
            {
                int count = Physics.OverlapSphereNonAlloc(enemy.EnemyMotor.GetPosition(), Range, m_targetColliders, m_mask, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < count; i++)
                {
                    if (m_targetColliders[i].TryGetComponent(out CombatEntity newTarget))
                    {
                        enemy.SetTarget(newTarget);
                        return;
                    }
                }
            }
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            if (user is UtilityEnemy enemy)
            {
                return enemy.HasTarget ? 0f : 10f;
            }
            else
            {
                return 0f;
            }
        }

        public override void OnDrawGizmos(CombatEntity user, CombatEntity target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(user.Motor.GetPosition(), Range);
        }
    }
}
