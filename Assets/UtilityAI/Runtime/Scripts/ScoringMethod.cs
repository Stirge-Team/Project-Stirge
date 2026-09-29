using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Stirge.Serialization;

    public abstract class ScoringMethod<T> where T : IScorable
    {
        protected T m_scorable;
        protected float m_scoreScaling;

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            return score * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        #region Setup
        private static TScoringMethod CreateInternal<TScoringMethod>(T scorable, float scoreScaling) where TScoringMethod : ScoringMethod<T>, new()
        {
            var scoringMethod = new TScoringMethod
            {
                m_scorable = scorable,
                m_scoreScaling = scoreScaling
            };
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod>(T scorable, float scoreScaling) where TScoringMethod : ScoringMethod<T>, INotSetupable, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg>(T scorable, TArg arg, float scoreScaling) where TScoringMethod : ScoringMethod<T>, ISetupable<TArg>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ0>(T scorable, TArg0 arg0, Targ0 arg1, float scoreScaling) where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, Targ0>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2>(T scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling) where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3>(T scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling) where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3, TArg4>(T scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling) where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3, arg4);
            return scoringMethod;
        }
        #endregion
    }
}
