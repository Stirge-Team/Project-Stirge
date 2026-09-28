using Stirge.Combat;
using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface IScoringMethod<in T> where T : IScorable
    {
        protected abstract float scoreScaling { set; }

        public float Evaluate(UtilityEnemy user, CombatEntity target);

        #region Setup
        public static TScoringMethod Create<TScoringMethod>(float scoreScaling) where TScoringMethod : IScoringMethod<T>, INotSetupable, new()
        {
            var scoringMethod = new TScoringMethod { scoreScaling = scoreScaling };
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg>(TArg arg, float scoreScaling) where TScoringMethod : IScoringMethod<T>, ISetupable<TArg>, new()
        {
            var scoringMethod = new TScoringMethod { scoreScaling = scoreScaling };
            scoringMethod.Setup(arg);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, float scoreScaling) where TScoringMethod : IScoringMethod<T>, ISetupable<TArg0, Targ0>, new()
        {
            var scoringMethod = new TScoringMethod { scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling) where TScoringMethod : IScoringMethod<T>, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var scoringMethod = new TScoringMethod { scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1, arg2);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling) where TScoringMethod : IScoringMethod<T>, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var scoringMethod = new TScoringMethod { scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1, arg2, arg3);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling) where TScoringMethod : IScoringMethod<T>, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var scoringMethod = new TScoringMethod { scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1, arg2, arg3, arg4);
            return scoringMethod;
        }
        #endregion
    }
}
