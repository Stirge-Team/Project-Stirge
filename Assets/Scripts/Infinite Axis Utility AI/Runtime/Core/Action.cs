using System;

namespace Stirge.UtilityAI.Core
{
    using Enemy;

    public abstract class Action
    {
        private Enemy m_enemy;

        public string name { get; set; }
        
        protected virtual void OnInitialise() { }

        protected virtual void OnBegin() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnEnd() { }
        
        public void Initialise()
        {
            OnInitialise();
        }

        public void Begin()
        {
            OnBegin();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void End()
        {
            OnEnd();
        }

        public void SetEnemy(Enemy enemy)
        {
            m_enemy = enemy;
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

        public static Action Create(Type actionType)
        {
            var action = (Action)Activator.CreateInstance(actionType);
            return action;
        }
        public static Action Create(Type actionType, object[] parameters)
        {
            var action = (Action)Activator.CreateInstance(actionType);
            SetuableHelper.CreateSetup(action, parameters);
            return action;
        }
        #endregion
    }
}
