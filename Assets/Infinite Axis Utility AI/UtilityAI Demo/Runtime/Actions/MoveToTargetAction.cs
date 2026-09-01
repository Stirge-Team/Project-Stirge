using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Actions
{
    using GenericBlackboard;
    using Core;
    using Stirge.Serialization;
    using UnityEngine.AI;

    public class MoveToTargetAction : Action, ISetupable<BlackboardPropertyName, BlackboardPropertyName>
    {
        private BlackboardPropertyName m_navMeshAgentPropertyName;
        private BlackboardPropertyName m_targetPropertyName;

        private NavMeshAgent m_agent;
        private DemoResource m_target;
        
        void ISetupable<BlackboardPropertyName, BlackboardPropertyName>.Setup(BlackboardPropertyName navMeshAgentPropertyName, BlackboardPropertyName targetPropertyName)
        {
            m_navMeshAgentPropertyName = navMeshAgentPropertyName;
            m_targetPropertyName = targetPropertyName;
        }
        
        protected override void OnInitialise()
        {
            Blackboard.TryGetClassValue(m_navMeshAgentPropertyName, out m_agent);
        }

        protected override void OnBegin()
        {
            if (Blackboard.TryGetClassValue(m_targetPropertyName, out m_target))
            {
                m_agent.isStopped = false;
                m_agent.SetDestination(m_target.transform.position);
            }
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnEnd()
        {
            if (m_agent != null)
                m_agent.isStopped = true;
        }
    }
}
