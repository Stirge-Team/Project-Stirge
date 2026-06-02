using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    public abstract class Action
    {
        internal string Name => GetType().Name;

        private Blackboard m_blackboard;
        
        protected virtual void OnInitialise() { }

        protected virtual void OnBegin() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnEnd() { }
        
        internal void Initialise()
        {
            OnInitialise();
        }

        internal void Begin()
        {
            OnBegin();
        }

        internal void Update()
        {
            OnUpdate();
        }

        internal void End()
        {
            OnEnd();
        }

        internal void SetBlackboard(Blackboard blackboard)
        {
            m_blackboard = blackboard;
        }

        #region Initialisers
        public static TAction Create<TAction>() where TAction : Action, INotSetupable, new()
        {
            var action = new TAction();
            return action;
        }
        public static TAction Create<TAction, TArg>(TArg arg) where TAction : Action, ISetupable<TArg>, new()
        {
            var action = new TAction();
            action.Setup(arg);
            return action;
        }
        public static TAction Create<TAction, TArg0, TArg1>(TArg0 arg0, TArg1 arg1) where TAction : Action, ISetupable<TArg0, TArg1>, new()
        {
            var action = new TAction();
            action.Setup(arg0, arg1);
            return action;
        }
        public static TAction Create<TAction, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2) where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            var action = new TAction();
            action.Setup(arg0, arg1, arg2);
            return action;
        }
        #endregion
    }
}
