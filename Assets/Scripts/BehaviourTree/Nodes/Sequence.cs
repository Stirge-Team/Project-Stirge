using System.Collections.Generic;
using UnityEngine;

namespace Stirge.AI
{
    public class Sequence : Node
    {
        public Sequence(List<Node> nodes)
        {
            m_nodes = nodes;
        }

        private List<Node> m_nodes = new();

        public override NodeStates Evaluate(Agent agent)
        {
            bool anyChildRunning = false;

            foreach (Node node in m_nodes)
            {
                switch (node.Evaluate(agent))
                {
                    case NodeStates.FAILURE:
                        m_nodeState = NodeStates.FAILURE;
                        return m_nodeState;
                    case NodeStates.SUCESS:
                        continue;
                    case NodeStates.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        m_nodeState = NodeStates.SUCESS;
                        return m_nodeState;
                }
            }

            m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCESS;
            return m_nodeState;
        }
    }
}
