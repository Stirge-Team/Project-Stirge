using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Actions
{
    using GenericBlackboard;
    using Core;
    using Stirge.Serialization;

    public class EatFoodAction : Action, ISetupable<BlackboardPropertyName>
    {
        private BlackboardPropertyName m_guyPropertyName;

        private Guy m_guy;
        
        void ISetupable<BlackboardPropertyName>.Setup(BlackboardPropertyName guyPropertyName)
        {
            m_guyPropertyName = guyPropertyName;
        }
        
        protected override void OnInitialise()
        {
            //Blackboard.TryGetClassValue(m_guyPropertyName, out m_guy);
        }

        protected override void OnBegin()
        {
            m_guy.BeginEatingFood();
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnEnd()
        {
            m_guy.StopAction();
        }
    }
}
