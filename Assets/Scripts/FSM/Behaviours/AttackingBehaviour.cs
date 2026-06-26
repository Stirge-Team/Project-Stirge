using UnityEngine;

namespace Stirge.AI
{
    using Combat.Attacks;
    using Combat.Attacks.Serialization;

    [System.Serializable]
    public class AttackingBehaviour : Behaviour
    {
        [SerializeField] private SerializedAttackData m_attackData;

        private AttackData m_deserializedAttackData;

        public override void _Enter(Agent agent)
        {
            if (m_deserializedAttackData == null)
                DeserializeAttackData();

            agent.Enemy.UseAttack(m_deserializedAttackData);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            // Nodes are being processed, so wait for the Attack to end
        }

        public override void _Exit(Agent agent)
        {
            agent.Enemy.StopAttacking();
        }

        public void DeserializeAttackData()
        {
            m_deserializedAttackData = m_attackData.CreateAttackData();
        }
    }
}
