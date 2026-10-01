using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;
    using System.Linq;

    /// <summary>
    /// Part of the <see cref="UtilityBrain"/>. Used to determine how the <see cref="UtilityEnemy"/> positions itself and moves in the world.<br/>
    /// When inheriting this class, you must also implement either <see cref="INotSetupable"/> or one of the <see cref="ISetupable{TArg}"/> interfaces.
    /// The number of generic arguments in <see cref="ISetupable{TArg}"/> should be equal to the number of unique properties you want your <see cref="MovementGoal"/> to have.
    /// </summary>
    public abstract class MovementGoal : IScorable
    {
        /// <summary>
        /// The returned score of this <see cref="MovementGoal"/> will be multiplied by this value.
        /// </summary>
        protected float m_scoreScaling = 1f;
        /// <summary>
        /// The length of time in seconds this <see cref="MovementGoal"/> will be performed for before the <see cref="UtilityBrain"/> will re-evaluate it's <see cref="MovementGoal"/>.
        /// </summary>
        protected float m_duration;
        /// <summary>
        /// The name of this <see cref="MovementGoal"/> to display for testing purposes.
        /// </summary>
        protected string m_displayName;
        /// <summary>
        /// The Conditions that must be met for this <see cref="MovementGoal"/> to be considered.
        /// </summary>
        protected ICondition[] m_conditions;
        /// <summary>
        /// The ScoringMethods that determine the score this <see cref="MovementGoal"/> will return when Evaluated.
        /// </summary>
        protected IScoringMethod[] m_scoringMethods;

        public float duration => m_duration;
        public string displayName => m_displayName;

        /// <inheritdoc/>
        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            // if the base score is Negative infinity, then we should only count the scores of the Scoring Methods
            if (score == Mathf.NegativeInfinity)
            {
                score = m_scoringMethods.Sum(s => s.Evaluate(user, target)) / m_scoringMethods.Length;
            }
            // otherwise, treat the base score as if it were an additional Scoring Method
            else
            {
                score += m_scoringMethods.Sum(s => s.Evaluate(user, target));
                score /= m_scoringMethods.Length + 1;
            }
            return score * m_scoreScaling;
        }

        /// <inheritdoc cref="IScorable.Evaluate"/>
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        /// <summary>
        /// What happens when the <paramref name="user"/> Performs this <see cref="MovementGoal"/>.
        /// </summary>
        public abstract void Perform(UtilityEnemy user, CombatEntity target);

        /// <summary>
        /// This method is called when this <see cref="MovementGoal"/> becomes the new <see cref="MovementGoal"/> to be Performed by the <see cref="UtilityBrain"/>.<br/>
        /// It should reset any values back to their defaults as necessary in case this <see cref="MovementGoal"/> has been run already.
        /// </summary>
        public abstract void Reset();

        #region Setup
        public void Setup(ICondition[] conditions, IScoringMethod[] scoringMethods)
        {
            m_conditions = conditions;
            m_scoringMethods = scoringMethods;
        }

        private static TMovementGoal CreateInternal<TMovementGoal>(float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, new()
        {
            var movementGoal = new TMovementGoal()
            {
                m_scoreScaling = scoreScaling,
                m_duration = duration,
                m_displayName = displayName
            };
            return movementGoal;
        }

        public static TMovementGoal Create<TMovementGoal>(float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, INotSetupable, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, displayName);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg>(TArg arg, float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, ISetupable<TArg>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, displayName);
            movementGoal.Setup(arg);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ0>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, displayName);
            movementGoal.Setup(arg0, arg1);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, displayName);
            movementGoal.Setup(arg0, arg1, arg2);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, displayName);
            movementGoal.Setup(arg0, arg1, arg2, arg3);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling, float duration, string displayName) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var movementGoal = CreateInternal<TMovementGoal>(scoreScaling, duration, displayName);
            movementGoal.Setup(arg0, arg1, arg2, arg3, arg4);
            return movementGoal;
        }
        #endregion
    }
}
