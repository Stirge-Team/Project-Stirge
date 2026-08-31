using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Stirge.Serialization;

    public abstract class ScoringMethod
    {
        protected float m_scoreScaling;
        protected Action m_action;

        public void Setup(Action action)
        {
            m_action = action;
        }

        public float Evaluate(CombatEntity user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            return score * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(CombatEntity user, CombatEntity target);

        #region Setup
        public static TScoringMethod Create<TScoringMethod>(float scoreScaling) where TScoringMethod : ScoringMethod, INotSetupable, new()
        {
            var scoringMethod = new TScoringMethod { m_scoreScaling = scoreScaling };
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg>(TArg arg, float scoreScaling) where TScoringMethod : ScoringMethod, ISetupable<TArg>, new()
        {
            var scoringMethod = new TScoringMethod { m_scoreScaling = scoreScaling };
            scoringMethod.Setup(arg);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, float scoreScaling) where TScoringMethod : ScoringMethod, ISetupable<TArg0, Targ0>, new()
        {
            var scoringMethod = new TScoringMethod { m_scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling) where TScoringMethod : ScoringMethod, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var scoringMethod = new TScoringMethod { m_scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1, arg2);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling) where TScoringMethod : ScoringMethod, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var scoringMethod = new TScoringMethod { m_scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1, arg2, arg3);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling) where TScoringMethod : ScoringMethod, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var scoringMethod = new TScoringMethod { m_scoreScaling = scoreScaling };
            scoringMethod.Setup(arg0, arg1, arg2, arg3, arg4);
            return scoringMethod;
        }
        #endregion
    }
}
