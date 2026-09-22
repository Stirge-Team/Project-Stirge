using Stirge.Serialization;
using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Serialized Action", fileName = "New Serialized Action", order = 449)]
    public class SerializedAction : ScriptableObject
    {
        [SerializeField, Range(0, 5f)] protected float m_scoreScaling = 1f;
        [SerializeField] protected float m_duration;
        [SerializeField] protected string m_displayName;
        [SerializeField] protected ActionType m_actionType;
        [SerializeField] protected TimelineAsset m_timeline;
        [SerializeField, Min(0)] protected float m_damage = 1f;
        [SerializeField, Min(0)] protected float m_range = 1f;
        [SerializeField] protected SerializedStatus_Base[] m_statuses = new SerializedStatus_Base[0];
        [SerializeField] protected SerializedCondition[] m_conditions = new SerializedCondition[0];
        [SerializeField] protected SerializedScoringMethod_Base[] m_scoringMethods = new SerializedScoringMethod_Base[0];

        public virtual Type actionType => typeof(Action);

        protected Status[] CreateRuntimeStatuses()
        {
            int statusCount = m_statuses.Length;
            Status[] statuses = new Status[statusCount];
            for (int i = 0; i < statusCount; i++)
            {
                statuses[i] = m_statuses[i].CreateRuntimeStatus();
            }
            return statuses;
        }

        protected ICondition[] CreateRuntimeConditions()
        {
            int conditionCount = m_conditions.Length;
            ICondition[] conditions = new ICondition[conditionCount];
            for (int i = 0; i < conditionCount; i++)
            {
                conditions[i] = m_conditions[i].CreateRuntimeCondition();
            }
            return conditions;
        }

        protected ScoringMethod[] CreateRuntimeScoringMethods()
        {
            int scoringMethodCount = m_scoringMethods.Length;
            ScoringMethod[] scoringMethods = new ScoringMethod[scoringMethodCount];
            for (int i = 0; i < scoringMethodCount; i++)
            {
                scoringMethods[i] = m_scoringMethods[i].CreateRuntimeScoringMethod();
            }
            return scoringMethods;
        }

        public virtual Action CreateRuntimeAction()
        {
            return Action.Create<Action>(m_scoreScaling, m_duration, m_displayName, m_actionType, m_timeline, m_damage, m_range, CreateRuntimeStatuses(), CreateRuntimeConditions(), CreateRuntimeScoringMethods());
        }
    }

    public abstract class SerializedAction<TAction, TArg> : SerializedAction where TAction : Action, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public override Type actionType => typeof(TAction);

        public sealed override Action CreateRuntimeAction()
        {
            return Action.Create<TAction, TArg>(m_arg, m_scoreScaling, m_duration, m_displayName, m_actionType, m_timeline, m_damage, m_range, CreateRuntimeStatuses(), CreateRuntimeConditions(), CreateRuntimeScoringMethods());
        }
    }

    public abstract class SerializedAction<TAction, TArg0, TArg1> : SerializedAction where TAction : Action, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public override Type actionType => typeof(TAction);

        public sealed override Action CreateRuntimeAction()
        {
            return Action.Create<TAction, TArg0, TArg1>(m_arg0, m_arg1, m_scoreScaling, m_duration, m_displayName, m_actionType, m_timeline, m_damage, m_range, CreateRuntimeStatuses(), CreateRuntimeConditions(), CreateRuntimeScoringMethods());
        }
    }

    public abstract class SerializedAction<TAction, TArg0, TArg1, TArg2> : SerializedAction where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public override Type actionType => typeof(TAction);

        public sealed override Action CreateRuntimeAction()
        {
            return Action.Create<TAction, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2, m_scoreScaling, m_duration, m_displayName, m_actionType, m_timeline, m_damage, m_range, CreateRuntimeStatuses(), CreateRuntimeConditions(), CreateRuntimeScoringMethods());
        }
    }

    public abstract class SerializedAction<TAction, TArg0, TArg1, TArg2, TArg3> : SerializedAction where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public sealed override Type actionType => typeof(TAction);

        public sealed override Action CreateRuntimeAction()
        {
            return Action.Create<TAction, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3, m_scoreScaling, m_duration, m_displayName, m_actionType, m_timeline, m_damage, m_range, CreateRuntimeStatuses(), CreateRuntimeConditions(), CreateRuntimeScoringMethods());
        }
    }

    public abstract class SerializedAction<TAction, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedAction where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(4)] private TArg4 m_arg4;

        public sealed override Type actionType => typeof(TAction);

        public sealed override Action CreateRuntimeAction()
        {
            return Action.Create<TAction, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4, m_scoreScaling, m_duration, m_displayName, m_actionType, m_timeline, m_damage, m_range, CreateRuntimeStatuses(), CreateRuntimeConditions(), CreateRuntimeScoringMethods());
        }
    }
}
