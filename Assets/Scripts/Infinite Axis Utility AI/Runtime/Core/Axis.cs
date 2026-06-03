using System;

namespace Stirge.UtilityAI.Core
{
    using Enemy;
    
    public abstract class Axis
    {
        private Enemy m_enemy;

        public string name { get; set; }
        public SerializableCallback<float> getValue { get; set; }

        public abstract float ComputeScore();

        protected virtual void OnInitialise() { }

        public void Initialise()
        {
            OnInitialise();
        }

        public void SetEnemy(Enemy enemy)
        {
            m_enemy = enemy;
        }

        #region Initialisers
        public static TAxis Create<TAxis>(SerializableCallback<float> getValue) where TAxis : Axis, INotSetupable, new()
        {
            var axis = new TAxis();
            axis.getValue = getValue;
            return axis;
        }
        public static TAxis Create<TAxis, TArg>(SerializableCallback<float> getValue, TArg arg) where TAxis : Axis, ISetupable<TArg>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg);
            return axis;
        }
        public static TAxis Create<TAxis, TArg0, Targ0>(SerializableCallback<float> getValue, TArg0 arg0, Targ0 arg1) where TAxis : Axis, ISetupable<TArg0, Targ0>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg0, arg1);
            axis.getValue = getValue;
            return axis;
        }
        public static TAxis Create<TAxis, TArg0, Targ1, Targ2>(SerializableCallback<float> getValue, TArg0 arg0, Targ1 arg1, Targ2 arg2) where TAxis : Axis, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var axis = new TAxis();
            axis.Setup(arg0, arg1, arg2);
            axis.getValue = getValue;
            return axis;
        }

        public static Axis Create(Type axisType, SerializableCallback<float> getValue)
        {
            var axis = (Axis)Activator.CreateInstance(axisType);
            axis.getValue = getValue;
            return axis;
        }
        public static Axis Create(Type axisType, object[] parameters, SerializableCallback<float> getValue)
        {
            var axis = (Axis)Activator.CreateInstance(axisType);
            SetuableHelper.CreateSetup(axis, parameters);
            return axis;
        }
        #endregion
    }
}
