using UnityEngine;

namespace Stirge.UtilityAI.Demo.Actions
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;

    public class SetLogTargetAction : Action, ISetupable<BlackboardPropertyName, BlackboardPropertyName, BlackboardPropertyName>
    {
        private BlackboardPropertyName m_transformPropertyName;
        private BlackboardPropertyName m_spawnerPropertyName;
        private BlackboardPropertyName m_targetPropertyName;

        private Transform m_transform;
        private ResourceSpawner m_spawner;
        
        void ISetupable<BlackboardPropertyName, BlackboardPropertyName, BlackboardPropertyName>.Setup(BlackboardPropertyName transformPropertyName,
            BlackboardPropertyName spawnerPropertyName, BlackboardPropertyName targetPropertyName)
        {
            m_transformPropertyName = transformPropertyName;
            m_spawnerPropertyName = spawnerPropertyName;
            m_targetPropertyName = targetPropertyName;
        }
        
        protected override void OnInitialise()
        {
            Blackboard.TryGetClassValue(m_transformPropertyName, out m_transform);
            Blackboard.TryGetClassValue(m_spawnerPropertyName, out m_spawner);
        }

        protected override void OnBegin()
        {
            
        }

        protected override void OnUpdate()
        {
            // get closest resource
            DemoResource newTarget = m_spawner.GetClosestLog(m_transform.position);
            Blackboard.SetClassValue(m_targetPropertyName, newTarget);
        }

        protected override void OnEnd()
        {
            
        }
    }
}
