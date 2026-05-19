using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class RangeNode : AttackNode
    {
        [System.Serializable]
        public class RangedNode
        {
            [SerializeReference]
            public AttackNode m_node;
            [SerializeField]
            private float m_range;
            public RangedNode(AttackNode node)
            {
                m_node = node;
            }
            public bool CheckRange(List<AttackNode> activeNodes, Vector3[] targetPositions)
            {
                if(Vector3.Distance(targetPositions[1], targetPositions[0]) <= m_range)
                {
                    m_node.Evaluate(activeNodes);
                    return true;
                }
                else return false;
            }
        }
        [SerializeReference]
        private RangedNode[] m_rangedNodes;
        private float m_range;
        private Vector3[] _targetPositions = new Vector3[2];
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            foreach(var node in m_rangedNodes)
            {
                if(node.CheckRange(activeNodes, _targetPositions))
                    return;
            }      
        }
        public void SetOrigin(Transform target)
        {
            _targetPositions[0] = target.position;
        }
        public void SetOrigin(Vector3 position)
        {
            _targetPositions[0] = position;
        }
        public void SetTarget(Transform target)
        {
            _targetPositions[1] = target.position;
        }
        public void SetTarget(Vector3 position)
        {
            _targetPositions[1] = position;
        }
    }
}
