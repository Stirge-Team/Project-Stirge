using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;
    using Stirge.Combat;
    using System;

    public abstract class Status
    {
        // fields
        protected float m_scoreScaling;
        protected StatusStackType m_stackType;
        protected StatusDurationType m_durationType;
        protected string m_displayName;
        protected int m_maxStacks;
        protected Condition[] m_conditions;

        // variables
        protected int m_currentStackCount;

        // properties
        public float scoreScaling => m_scoreScaling;
        public StatusStackType stackType => m_stackType;
        public StatusDurationType durationType => m_durationType;
        public string displayName => m_displayName;
        public int maxStacks => m_maxStacks;
        public Condition[] conditions => m_conditions;

        public int currentStackCount
        {
            get => m_currentStackCount;
            set => m_currentStackCount = value;
        }

        public abstract Type statusType { get; }

        public float Evaluate(CombatEntity user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            return score * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(CombatEntity user, CombatEntity target);

        /// <summary>
        /// User is passed here so it may be saved as a reference for any effects that require the applier of the effect during Resolve or Clear.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns>If the Status should end.</returns>
        public abstract bool OnApply(CombatEntity user, CombatEntity target);
        /// <summary>
        /// Update method.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>If the Status should end.</returns>
        public abstract bool Update(CombatEntity target);
        /// <summary>
        /// Run before removing the Status from the Statuses array.
        /// </summary>
        /// <param name="target"></param>
        public abstract void OnClear(CombatEntity target);

        #region Setup
        public static TStatus Create<TStatus>() where TStatus : Status, INotSetupable, new()
        {
            var status = new TStatus();
            return status;
        }
        public static TStatus Create<TStatus, TArg>(TArg arg) where TStatus : Status, ISetupable<TArg>, new()
        {
            var status = new TStatus();
            status.Setup(arg);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ0>(TArg0 arg0, Targ0 arg1) where TStatus : Status, ISetupable<TArg0, Targ0>, new()
        {
            var status = new TStatus();
            status.Setup(arg0, arg1);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var status = new TStatus();
            status.Setup(arg0, arg1, arg2);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var status = new TStatus();
            status.Setup(arg0, arg1, arg2, arg3);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var status = new TStatus();
            status.Setup(arg0, arg1, arg2, arg3, arg4);
            return status;
        }
        #endregion
    }
}
