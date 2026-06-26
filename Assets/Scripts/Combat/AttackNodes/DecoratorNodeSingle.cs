using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public abstract class DecoratorNodeSingle : AttackNode, IDecoratorNode, Stirge.Serialization.ISetupable<AttackNode>
    {
        [SerializeField] protected AttackNode m_node;

        public void Setup(AttackNode node)
        {
            m_node = node;
        }

        void IDecoratorNode.AddAttackNode(AttackNode attackNode)
        {
            if (m_node != null)
            {
                Debug.LogError($"This Single Decorator Node: {name} already has a bound Attack Node but a new one is being assigned! Your SerializedAttackData is borked!!");
                return;
            }
            m_node = attackNode;
        }
    }
}
