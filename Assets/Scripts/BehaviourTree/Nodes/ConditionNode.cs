using UnityEngine;

namespace Stirge.AI
{
    public class ConditionNode : Node
    {
        public ConditionNode(ConditionDelegate condition)
        {
            m_condition = condition;
        }

        public delegate NodeStates ConditionDelegate(Agent agent);
        private ConditionDelegate m_condition;

        public override NodeStates Evaluate(Agent agent)
        {
            switch (m_condition(agent))
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCESS:
                    m_nodeState = NodeStates.SUCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
            }
        }
    }
}
