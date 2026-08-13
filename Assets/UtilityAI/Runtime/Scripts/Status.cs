using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;
    using Stirge.Combat;
    using System;

    public abstract class Status
    {
        // constants
        protected string m_displayName;
        protected StatusStackType m_stackType;
        protected StatusDurationType m_durationType;
        protected int m_maxStackCount;

        public string displayName => m_displayName;
        public StatusStackType stackType => m_stackType;
        public StatusDurationType durationType => m_durationType;
        public int maxStacks => m_maxStackCount;

        // variables
        protected int m_currentStackCount;
        
        public int currentStackCount
        {
            get => m_currentStackCount;
            set => m_currentStackCount = value;
        }

        public abstract Type statusType { get; }

        public void Init(SerializedStatus_Base serializedStatus)
        {
            m_displayName = serializedStatus.displayName;
            m_durationType = serializedStatus.durationType;
            m_maxStackCount = serializedStatus.maxStacks;
        }

        /// <summary>
        /// User is passed here so it may be saved as a reference for any effects that require the applier of the effect during Resolve or Clear.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns>If the Status should end.</returns>
        public abstract bool Apply(CombatEntity user, CombatEntity target);
        /// <summary>
        /// Update method.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>If the Status should end.</returns>
        public abstract bool Resolve(CombatEntity target);
        /// <summary>
        /// Run before removing the Status from the Statuses array.
        /// </summary>
        /// <param name="target"></param>
        public abstract void Clear(CombatEntity target);

        public abstract float Evaluate(CombatEntity user, CombatEntity target);

        #region Setup
        public static TStatus Create<TStatus>(SerializedStatus_Base serializedStatus) where TStatus : Status, INotSetupable, new()
        {
            var status = new TStatus();
            status.Init(serializedStatus);
            return status;
        }
        public static TStatus Create<TStatus, TArg>(SerializedStatus_Base serializedStatus, TArg arg) where TStatus : Status, ISetupable<TArg>, new()
        {
            var status = new TStatus();
            status.Init(serializedStatus);
            status.Setup(arg);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ0>(SerializedStatus_Base serializedStatus, TArg0 arg0, Targ0 arg1) where TStatus : Status, ISetupable<TArg0, Targ0>, new()
        {
            var status = new TStatus();
            status.Init(serializedStatus);
            status.Setup(arg0, arg1);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2>(SerializedStatus_Base serializedStatus, TArg0 arg0, Targ1 arg1, Targ2 arg2) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var status = new TStatus();
            status.Init(serializedStatus);
            status.Setup(arg0, arg1, arg2);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3>(SerializedStatus_Base serializedStatus, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var status = new TStatus();
            status.Init(serializedStatus);
            status.Setup(arg0, arg1, arg2, arg3);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3, TArg4>(SerializedStatus_Base serializedStatus, TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var status = new TStatus();
            status.Init(serializedStatus);
            status.Setup(arg0, arg1, arg2, arg3, arg4);
            return status;
        }
        #endregion
    }
}
