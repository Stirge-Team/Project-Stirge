using System;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI.Core
{
    using Stirge.Serialization;

    public abstract class Axis
    {
        private Blackboard m_blackboard;

        public string name { get; set; }
        public Blackboard blackboard => m_blackboard;

        public abstract float ComputeScore();

        protected virtual void OnInitialise() { }

        public void Initialise()
        {
            OnInitialise();
        }

        public void SetBlackboard(Blackboard blackboard)
        {
            m_blackboard = blackboard;
        }

        #region Initialisers
        public static TAxis Create<TAxis>() where TAxis : Axis, INotSetupable, new()
        {
            var axis = new TAxis();
            return axis;
        }
        public static TAxis Create<TAxis, TArg>(TArg arg) where TAxis : Axis, ISetupable<TArg>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg);
            return axis;
        }
        public static TAxis Create<TAxis, TArg0, Targ0>(TArg0 arg0, Targ0 arg1) where TAxis : Axis, ISetupable<TArg0, Targ0>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg0, arg1);
            return axis;
        }
        public static TAxis Create<TAxis, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2) where TAxis : Axis, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg0, arg1, arg2);
            return axis;
        }

        public static Axis Create(Type axisType)
        {
            var axis = (Axis)Activator.CreateInstance(axisType);
            return axis;
        }
        public static Axis Create(Type axisType, object[] parameters)
        {
            var axis = (Axis)Activator.CreateInstance(axisType);
            SetuableHelper.CreateSetup(axis, parameters);
            return axis;
        }
        #endregion
    }
}
