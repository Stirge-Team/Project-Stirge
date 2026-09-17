using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;
    using System.Linq;

    public abstract class MovementGoal
    {
        protected Action m_action;

        protected float m_scoreScaling = 1f;
        protected float m_duration;
        protected ICondition[] m_conditions;
        protected ScoringMethod[] m_scoringMethods;

        public float Evaluate(CombatEntity user, CombatEntity target)
        {
            float baseScore = (EvaluateInternal(user, target) + m_scoringMethods.Sum(s => s.Evaluate(user, target))) / (m_scoringMethods.Length + 1);
            return baseScore * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(CombatEntity user, CombatEntity target);

        public void Setup(Action action)
        {
            m_action = action;
        }

        #region Setup
        private static TMovementGoal CreateInternal<TMovementGoal>(float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, new()
        {
            var movementGoal = new TMovementGoal()
            {
                m_scoreScaling = scoreScaling,
                m_duration = duration,
                m_conditions = conditions,
                m_scoringMethods = scoringMethods
            };
            return movementGoal;
        }

        public static TMovementGoal Create<TMovementGoal>(float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, INotSetupable, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, conditions, scoringMethods);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg>(TArg arg, float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, ISetupable<TArg>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, conditions, scoringMethods);
            movementGoal.Setup(arg);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ0>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, conditions, scoringMethods);
            movementGoal.Setup(arg0, arg1);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, conditions, scoringMethods);
            movementGoal.Setup(arg0, arg1, arg2);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, conditions, scoringMethods);
            movementGoal.Setup(arg0, arg1, arg2, arg3);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling, float duration, ICondition[] conditions, ScoringMethod[] scoringMethods) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, conditions, scoringMethods);
            movementGoal.Setup(arg0, arg1, arg2, arg3, arg4);
            return movementGoal;
        }
        #endregion
    }
}
