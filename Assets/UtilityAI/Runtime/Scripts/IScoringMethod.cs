using Stirge.Combat;
using Stirge.Serialization;
using System.Dynamic;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface IScoringMethod<in TScorable> where TScorable : IScorable
    {
        protected abstract TScorable scorable { set; }
        protected abstract float scoreScaling { set; }

        public float Evaluate(UtilityEnemy user, CombatEntity target);

        #region Setup
        private static TScoringMethod CreateInternal<TScoringMethod>(TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, new()
        {
            var scoringMethod = new TScoringMethod
            {
                scorable = scorable,
                scoreScaling = scoreScaling
            };
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod>(TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, INotSetupable, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg>(TScorable scorable, TArg arg, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, ISetupable<TArg>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ0>(TScorable scorable, TArg0 arg0, Targ0 arg1, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, ISetupable<TArg0, Targ0>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2>(TScorable scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3>(TScorable scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3, TArg4>(TScorable scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling) where TScoringMethod : IScoringMethod<TScorable>, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3, arg4);
            return scoringMethod;
        }
        #endregion
    }
}
