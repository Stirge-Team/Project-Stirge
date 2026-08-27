using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;

    public abstract class SerializedMovementGoal<TMovementGoal> : SerializedMovementGoal_Base where TMovementGoal : MovementGoal, INotSetupable, new()
    {
        public override Type movementGoalType => typeof(TMovementGoal);

        public sealed override MovementGoal CreateRuntimeMovementGoal()
        {
            return MovementGoal.Create<TMovementGoal>();
        }
    }
    public abstract class SerializedMovementGoal<TMovementGoal, TArg> : SerializedMovementGoal_Base where TMovementGoal : MovementGoal, ISetupable<TArg>, new()
    {
        [Header("Movement Goal Properties")]
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public override Type movementGoalType => typeof(TMovementGoal);

        public sealed override MovementGoal CreateRuntimeMovementGoal()
        {
            return MovementGoal.Create<TMovementGoal, TArg>(m_arg);
        }
    }
    public abstract class SerializedMovementGoal<TMovementGoal, TArg0, TArg1> : SerializedMovementGoal_Base where TMovementGoal : MovementGoal, ISetupable<TArg0, TArg1>, new()
    {
        [Header("Movement Goal Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public override Type movementGoalType => typeof(TMovementGoal);

        public sealed override MovementGoal CreateRuntimeMovementGoal()
        {
            return MovementGoal.Create<TMovementGoal, TArg0, TArg1>(m_arg0, m_arg1);
        }
    }

    public abstract class SerializedMovementGoal<TMovementGoal, TArg0, TArg1, TArg2> : SerializedMovementGoal_Base where TMovementGoal : MovementGoal, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [Header("Movement Goal Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public override Type movementGoalType => typeof(TMovementGoal);

        public sealed override MovementGoal CreateRuntimeMovementGoal()
        {
            return MovementGoal.Create<TMovementGoal, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2);
        }
    }

    public abstract class SerializedMovementGoal<TMovementGoal, TArg0, TArg1, TArg2, TArg3> : SerializedMovementGoal_Base where TMovementGoal : MovementGoal, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [Header("Movement Goal Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public sealed override Type movementGoalType => typeof(TMovementGoal);

        public sealed override MovementGoal CreateRuntimeMovementGoal()
        {
            return MovementGoal.Create<TMovementGoal, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3);
        }
    }

    public abstract class SerializedMovementGoal<TMovementGoal, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedMovementGoal_Base where TMovementGoal : MovementGoal, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [Header("Movement Goal Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(4)] private TArg4 m_arg4;

        public sealed override Type movementGoalType => typeof(TMovementGoal);

        public sealed override MovementGoal CreateRuntimeMovementGoal()
        {
            return MovementGoal.Create<TMovementGoal, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4);
        }
    }
}
