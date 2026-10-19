using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;

    /// <summary>
    /// Attached to an <see cref="Action"/> or <see cref="MovementGoal"/> to determine if it can be Evaluated by the <see cref="UtilityBrain"/>.<br/>
    /// Attached to a <see cref="Status"/> to determine if it can be inflicted by an Attack.<br/><br/>
    /// When inheriting this class, you must provide a type for the generic argument <see cref="TScorable"/>. It must be either:<br/>
    /// -    <see cref="IScorable"/> to allow this to be used on any type of AI Object,<br/>
    /// -    Or, you can use <see cref="Action"/>, <see cref="MovementGoal"/>, or <see cref="Status"/> to be able to reference the properties of that object in your Evaluate() override.
    /// <br/><br/>
    /// You must also implement either <see cref="INotSetupable"/> or one of the <see cref="ISetupable{TArg}"/> interfaces.
    /// The number of generic arguments in <see cref="ISetupable{TArg}"/> should be equal to the number of unique properties you want your <see cref="Condition{TScorable}"/> to have.
    /// </summary>
    /// <typeparam name="TScorable">Must be <see cref="Action"/>, <see cref="MovementGoal"/>, <see cref="Status"/>, or <see cref="IScorable"/>.</typeparam>

    public abstract class Condition<TScorable> : ICondition where TScorable : class, IScorable
    {
        protected TScorable m_scorable;

        public abstract bool Evaluate(CombatEntity user, CombatEntity target);

        #region Setup
        public void Setup<T>(T scorable) where T : class, IScorable
        {
            m_scorable = scorable as TScorable;
        }

        private static TCondition CreateInternal<TCondition>(TScorable scorable) where TCondition : ICondition, new()
        {
            var condition = new TCondition();
            condition.Setup(scorable);
            return condition;
        }
        public static TCondition Create<TCondition>(TScorable scorable) where TCondition : ICondition, INotSetupable, new()
        {
            var condition = CreateInternal<TCondition>(scorable);
            return condition;
        }
        public static TCondition Create<TCondition, TArg>(TArg arg, TScorable scorable) where TCondition : ICondition, ISetupable<TArg>, new()
        {
            var condition = CreateInternal<TCondition>(scorable);
            condition.Setup(arg);
            return condition;
        }
        public static TCondition Create<TCondition, TArg0, TArg1>(TArg0 arg0, TArg1 arg1, TScorable scorable) where TCondition : ICondition, ISetupable<TArg0, TArg1>, new()
        {
            var condition = CreateInternal<TCondition>(scorable);
            condition.Setup(arg0, arg1);
            return condition;
        }
        public static TCondition Create<TCondition, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TScorable scorable) where TCondition : ICondition, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            var condition = CreateInternal<TCondition>(scorable);
            condition.Setup(arg0, arg1, arg2);
            return condition;
        }
        public static TCondition Create<TCondition, TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TScorable scorable) where TCondition : ICondition, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
        {
            var condition = CreateInternal<TCondition>(scorable);
            condition.Setup(arg0, arg1, arg2, arg3);
            return condition;
        }
        public static TCondition Create<TCondition, TArg0, TArg1, TArg2, TArg3, TArg4>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TScorable scorable) where TCondition : ICondition, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
        {
            var condition = CreateInternal<TCondition>(scorable);
            condition.Setup(arg0, arg1, arg2, arg3, arg4);
            return condition;
        }
        #endregion
    }
}
