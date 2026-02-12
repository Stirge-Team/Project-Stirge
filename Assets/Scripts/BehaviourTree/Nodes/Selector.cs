using System.Collections.Generic;
using UnityEngine;

namespace Stirge.AI
{
    public class Selector : Node
    {
        public Selector(List<Node> nodes)
        {
            m_nodes = nodes;
        }

        protected List<Node> m_nodes = new();

        public override NodeStates Evaluate(Agent agent)
        {
            foreach (Node node in m_nodes)
            {
                switch (node.Evaluate(agent))
                {
                    case NodeStates.FAILURE:
                        continue;
                    case NodeStates.SUCESS:
                        m_nodeState=NodeStates.SUCESS;
                        return m_nodeState;
                    case NodeStates.RUNNING:
                        m_nodeState = NodeStates.RUNNING;
                        return m_nodeState;
                    default:
                        continue;
                }
            }

            m_nodeState = NodeStates.FAILURE;
            return m_nodeState;
        }
    }
}
