using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Stirge.Serialization;

    public interface IScoringMethod
    {
        protected float scoreScaling { get; }

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float score = EvaluateInterface(user, target);
            return score * scoreScaling;
        }
        protected abstract float EvaluateInterface(UtilityEnemy user, CombatEntity target);

        public abstract void Setup<T>(T scorable, float scoreScaling) where T : class, IScorable;
    }

    public abstract class ScoringMethod<TScorable> : IScoringMethod where TScorable : class, IScorable
    {
        protected TScorable m_scorable;
        protected float m_scoreScaling;

        float IScoringMethod.scoreScaling => m_scoreScaling;

        float IScoringMethod.EvaluateInterface(UtilityEnemy user, CombatEntity target)
        {
            return EvaluateInternal(user, target);
        }
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        public void Setup<T>(T scorable, float scoreScaling) where T : class, IScorable
        {
            m_scorable = scorable as TScorable;
            m_scoreScaling = scoreScaling;
        }

        #region Setup
        private static TScoringMethod CreateInternal<TScoringMethod>(TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, new()
        {
            var scoringMethod = new TScoringMethod();
            scoringMethod.Setup(scorable, scoreScaling);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod>(TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, INotSetupable, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg>(TScorable scorable, TArg arg, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ0>(TScorable scorable, TArg0 arg0, Targ0 arg1, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ0>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2>(TScorable scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3>(TScorable scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3, TArg4>(TScorable scorable, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3, arg4);
            return scoringMethod;
        }
        #endregion
    }
}
