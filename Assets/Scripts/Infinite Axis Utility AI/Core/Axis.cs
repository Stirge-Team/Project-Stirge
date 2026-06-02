using UnityEngine;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    public abstract class Axis
    {
        private Blackboard m_blackboard;

        public string Name => GetType().Name;

        public abstract float ComputeScore();

        protected virtual void OnInitialise() { }

        internal void Initialise()
        {
            OnInitialise();
        }

        internal void SetBlackboard(Blackboard blackboard)
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
        public static TAxis Create<TAxis, TArg0, Targ0, Targ1>(TArg0 arg0, Targ0 arg1, Targ1 arg2) where TAxis : Axis, ISetupable<TArg0, Targ0, Targ1>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg0, arg1, arg2);
            return axis;
        }
        #endregion
    }
}
