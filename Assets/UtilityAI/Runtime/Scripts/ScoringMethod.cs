using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;

    /// <summary>
    /// Attached to an <see cref="Action"/>, <see cref="MovementGoal"/>, or <see cref="Status"/> to modify the score it returns when Evaluated by the <see cref="UtilityBrain"/>.<br/><br/>
    /// When inheriting this class, you must provide a type for the generic argument <see cref="TScorable"/>. It must be either:<br/>
    /// -    <see cref="IScorable"/> to allow this to be used on any type of AI Object,<br/>
    /// -    Or, you can use <see cref="Action"/>, <see cref="MovementGoal"/>, or <see cref="Status"/> to be able to reference the properties of that object in your EvaluateInternal() override.
    /// <br/><br/>
    /// You must also implement either <see cref="INotSetupable"/> or one of the <see cref="ISetupable{TArg}"/> interfaces.
    /// The number of generic arguments in <see cref="ISetupable{TArg}"/> should be equal to the number of unique properties you want your <see cref="ScoringMethod{TScorable}"/> to have.
    /// </summary>
    /// <typeparam name="TScorable">Must be <see cref="Action"/>, <see cref="MovementGoal"/>, <see cref="Status"/>, or <see cref="IScorable"/>.</typeparam>
    public abstract class ScoringMethod<TScorable> : IScoringMethod where TScorable : class, IScorable
    {
        /// <summary>
        /// The <see cref="Action"/>, <see cref="MovementGoal"/>, or <see cref="Status"/> that this <see cref="ScoringMethod{TScorable}"/> is part of.
        /// </summary>
        protected TScorable m_scorable;
        /// <summary>
        /// The returned score of this <see cref="ScoringMethod{TScorable}"/> will be multiplied by this value.
        /// </summary>
        protected float m_scoreScaling;

        /// <summary>
        /// Returns the score for this <see cref="ScoringMethod{TScorable}"/>.<br/>
        /// </summary>
        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            return score * m_scoreScaling;
        }

        /// <summary>
        /// Override this to implement your own <see cref="ScoringMethod{TScorable}"/>.
        /// </summary>
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        #region Setup
        public void Setup<T>(T scorable, float scoreScaling) where T : class, IScorable
        {
            m_scorable = scorable as TScorable;
            m_scoreScaling = scoreScaling;
        }

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
        public static TScoringMethod Create<TScoringMethod, TArg>(TArg arg, TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ0>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3);
            return scoringMethod;
        }
        public static TScoringMethod Create<TScoringMethod, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, TScorable scorable, float scoreScaling) where TScoringMethod : IScoringMethod, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var scoringMethod = CreateInternal<TScoringMethod>(scorable, scoreScaling);
            scoringMethod.Setup(arg0, arg1, arg2, arg3, arg4);
            return scoringMethod;
        }
        #endregion
    }
}
