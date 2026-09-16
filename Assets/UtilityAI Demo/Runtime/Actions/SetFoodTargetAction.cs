using UnityEngine;

namespace Stirge.UtilityAI.Demo.Actions
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;

    public class SetFoodTargetAction : Action, ISetupable<BlackboardPropertyName, BlackboardPropertyName, BlackboardPropertyName>
    {
        private BlackboardPropertyName m_transformPropertyName;
        private BlackboardPropertyName m_targetPropertyName;
        private BlackboardPropertyName m_spawnerPropertyName;

        private Transform m_transform;
        private ResourceSpawner m_spawner;
        
        void ISetupable<BlackboardPropertyName, BlackboardPropertyName, BlackboardPropertyName>.Setup(BlackboardPropertyName transformPropertyName,
            BlackboardPropertyName targetPropertyName, BlackboardPropertyName spawnerPropertyName)
        {
            m_transformPropertyName = transformPropertyName;
            m_targetPropertyName = targetPropertyName;
            m_spawnerPropertyName = spawnerPropertyName;
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
            DemoResource newTarget = m_spawner.GetClosestFood(m_transform.position);
            Blackboard.SetClassValue(m_targetPropertyName, newTarget);
        }

        protected override void OnEnd()
        {
            
        }
    }
}
