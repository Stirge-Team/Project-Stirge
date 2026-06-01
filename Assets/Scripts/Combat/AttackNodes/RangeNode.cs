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
            public float Range => m_range;
            public RangedNode(AttackNode node)
            {
                m_node = node;
            }
            public bool CheckRange(List<AttackNode> activeNodes, Vector3[] targetPositions)
            {
                if (Vector3.Distance(targetPositions[1], targetPositions[0]) <= m_range)
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
            //sorting the attack nodes so they work based on range correctly every time.
            bool sorted = false;
            RangedNode[] sortedNodes = m_rangedNodes; //copied the array - would it be worth sorting the main array? idk if it gets "reset" to a default.
            while (!sorted) for (int escapeValue = 128; escapeValue > 0; escapeValue--)  //for loop for safety
            {
                sorted = true;
                if (escapeValue > 0)
                    for (int i = 1; i < m_rangedNodes.Length; i++)
                    {
                        if (sortedNodes[i - 1].Range > sortedNodes[i].Range) //range before greater then now
                        {
                            RangedNode tmp = sortedNodes[i - 1]; //save
                            sortedNodes[i - 1] = sortedNodes[i]; //overwrite
                            sortedNodes[i] = tmp; //restore
                            sorted = false; // we need to check the array is sorted now.
                        }
                    }
            }
            foreach (var node in sortedNodes)
            {
                if (node.CheckRange(activeNodes, _targetPositions))
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
