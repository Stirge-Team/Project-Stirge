using UnityEngine;

namespace Stirge.AI
{
    public class Inverter : Node
    {
        public Inverter(Node node)
        {
            m_node = node;
        }

        private Node m_node;
        public Node node => m_node;

        public override NodeStates Evaluate(Agent agent)
        {
            switch (m_node.Evaluate(agent))
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.SUCESS;
                    return m_nodeState;
                case NodeStates.SUCESS:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
            }

            m_nodeState = NodeStates.SUCESS;
            return m_nodeState;
        }
    }
}
