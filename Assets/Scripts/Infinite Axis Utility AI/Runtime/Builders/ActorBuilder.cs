using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI.Builders
{
    using Core;
    using Stirge.Serialization;

    public class ActorBuilder
    {
        [SerializeField] private readonly List<IActionBuilder> m_actionBuilders = new();
        [SerializeField] private readonly List<IAxisBuilder> m_axisBuilders = new();
        [SerializeField] private readonly List<List<int>> m_actionAxisBindings = new();

        private readonly List<int>[] m_fastAxesLookup = new List<int>[9]
        {
            new(), new(), new(), new(), new(), new(), new(), new(), new()
        };

        private readonly object[][] m_parametersCache = new object[3][]
        {
            new object[1], new object[2], new object[3]
        };

        public Actor Build(UtilityEnemy enemy)
        {
            Axis[] axes = MakeAxes();
            Action[] actions = MakeActions();
            int[][] actionAxisBindings = MakeActionAxisBindings();

            Actor actor = Actor.Create(enemy, axes, actions, actionAxisBindings);

            return actor;
        }

        #region Action Adders
        public void AddAction<TAction>(string name = "") where TAction : Action, INotSetupable, new()
        {
            m_actionBuilders.Add(new ActionBuilder<TAction>(name));
            m_actionAxisBindings.Add(new List<int>());
        }
        public void AddAction<TAction, TArg>(TArg arg, string name = "") where TAction : Action, ISetupable<TArg>, new()
        {
            m_actionBuilders.Add(new ActionBuilder<TAction, TArg>(arg, name));
            m_actionAxisBindings.Add(new List<int>());
        }
        public void AddAction<TAction, TArg0, TArg1>(TArg0 arg0, TArg1 arg1, string name = "") where TAction : Action, ISetupable<TArg0, TArg1>, new()
        {
            m_actionBuilders.Add(new ActionBuilder<TAction, TArg0, TArg1>(arg0, arg1, name));
            m_actionAxisBindings.Add(new List<int>());
        }
        public void AddAction<TAction, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2, string name = "") where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            m_actionBuilders.Add(new ActionBuilder<TAction, TArg0, TArg1, TArg2>(arg0, arg1, arg2, name));
            m_actionAxisBindings.Add(new List<int>());
        }
        public void AddAction(Type actionType, string name = "", params object[] parameters)
        {
            if (parameters is { Length: > 3 })
            {
                throw new ArgumentException($"Failed to add an action of type {actionType}. Too many {nameof(parameters)} were passed. The method supports up to 3.");
            }

            m_actionBuilders.Add(new ActionBuilder(actionType, parameters, name));
            m_actionAxisBindings.Add(new List<int>());
        }
        #endregion

        #region Axis Adders
        public void AddAxis<TAxis>(string name = "") where TAxis : Axis, INotSetupable, new()
        {
            if (m_actionAxisBindings.Count == 0)
			{
				throw new InvalidOperationException($"Failed to add an axis of type {typeof(TAxis)}. No action was added before.");
			}

			const int parametersCount = 0;
			List<int> axesLookup = m_fastAxesLookup[parametersCount];
			int sameIndex = -1;

			for (int i = 0, count = axesLookup.Count; i < count; ++i)
			{
				int axisIndex = axesLookup[i];
				IAxisBuilder axisBuilder = m_axisBuilders[axisIndex];

				if (axisBuilder is AxisBuilder<TAxis>)
				{
					sameIndex = axisIndex;
					break;
				}

				if (axisBuilder is AxisBuilder nonGenericAxisBuilder &&
					nonGenericAxisBuilder.axisType == typeof(TAxis))
				{
					sameIndex = axisIndex;
					break;
				}
			}

			if (sameIndex >= 0)
			{
				m_actionAxisBindings[^1].Add(sameIndex);
			}
			else
			{
				int count = m_axisBuilders.Count;
				m_actionAxisBindings[^1].Add(count);
				axesLookup.Add(count);
				m_axisBuilders.Add(new AxisBuilder<TAxis>(name));
			}
        }
        public void AddAxis<TAxis, TArg>(TArg arg, string name = "") where TAxis : Axis, ISetupable<TArg>, new()
        {
            if (m_actionAxisBindings.Count == 0)
            {
                throw new InvalidOperationException($"Failed to add an axis of type {typeof(TAxis)}. No action was added before.");
            }

            const int parametersCount = 1;
            List<int> axesLookup = m_fastAxesLookup[parametersCount];
            int sameIndex = -1;

            for (int i = 0, count = axesLookup.Count; i < count; ++i)
            {
                int axisIndex = axesLookup[i];
                IAxisBuilder axisBuilder = m_axisBuilders[axisIndex];

                if (axisBuilder is AxisBuilder<TAxis, TArg> genericAxisBuilder)
                {
                    if (genericAxisBuilder.AreEqual(arg))
                    {
                        sameIndex = axisIndex;
                        break;
                    }
                }
                else if (axisBuilder is AxisBuilder nonGenericAxisBuilder &&
                        nonGenericAxisBuilder.axisType == typeof(TAxis))
                {
                    const int parametersCacheIndex = parametersCount - 1;
                    object[] parametersCache = m_parametersCache[parametersCacheIndex];
                    parametersCache[0] = arg;

                    if (nonGenericAxisBuilder.AreEqual(parametersCache))
                    {
                        sameIndex = axisIndex;
                        Array.Clear(parametersCache, 0, parametersCount);
                        break;
                    }

                    Array.Clear(parametersCache, 0, parametersCount);
                }
            }

            if (sameIndex >= 0)
            {
                m_actionAxisBindings[^1].Add(sameIndex);
            }
            else
            {
                int count = m_axisBuilders.Count;
                m_actionAxisBindings[^1].Add(count);
                axesLookup.Add(count);
                m_axisBuilders.Add(new AxisBuilder<TAxis, TArg>(arg, name));
            }
        }
        public void AddAxis<TAxis, TArg0, TArg1>(TArg0 arg0, TArg1 arg1, string name = "") where TAxis : Axis, ISetupable<TArg0, TArg1>, new()
        {
            if (m_actionAxisBindings.Count == 0)
            {
                throw new InvalidOperationException($"Failed to add an axis of type {typeof(TAxis)}. No action was added before.");
            }

            const int parametersCount = 2;
            List<int> axesLookup = m_fastAxesLookup[parametersCount];
            int sameIndex = -1;

            for (int i = 0, count = axesLookup.Count; i < count; ++i)
            {
                int axisIndex = axesLookup[i];
                IAxisBuilder axisBuilder = m_axisBuilders[axisIndex];

                if (axisBuilder is AxisBuilder<TAxis, TArg0, TArg1> genericAxisBuilder)
                {
                    if (genericAxisBuilder.AreEqual(arg0, arg1))
                    {
                        sameIndex = axisIndex;
                        break;
                    }
                }
                else if (axisBuilder is AxisBuilder nonGenericAxisBuilder &&
                        nonGenericAxisBuilder.axisType == typeof(TAxis))
                {
                    const int parametersCacheIndex = parametersCount - 1;
                    object[] parametersCache = m_parametersCache[parametersCacheIndex];
                    parametersCache[0] = arg0;
                    parametersCache[1] = arg1;

                    if (nonGenericAxisBuilder.AreEqual(parametersCache))
                    {
                        sameIndex = axisIndex;
                        Array.Clear(parametersCache, 0, parametersCount);
                        break;
                    }

                    Array.Clear(parametersCache, 0, parametersCount);
                }
            }

            if (sameIndex >= 0)
            {
                m_actionAxisBindings[^1].Add(sameIndex);
            }
            else
            {
                int count = m_axisBuilders.Count;
                m_actionAxisBindings[^1].Add(count);
                axesLookup.Add(count);
                m_axisBuilders.Add(new AxisBuilder<TAxis, TArg0, TArg1>(arg0, arg1, name));
            }
        }
        public void AddAxis<TAxis, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2, string name = "") where TAxis : Axis, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            if (m_actionAxisBindings.Count == 0)
            {
                throw new InvalidOperationException($"Failed to add an axis of type {typeof(TAxis)}. No action was added before.");
            }

            const int parametersCount = 3;
            List<int> axesLookup = m_fastAxesLookup[parametersCount];
            int sameIndex = -1;

            for (int i = 0, count = axesLookup.Count; i < count; ++i)
            {
                int axisIndex = axesLookup[i];
                IAxisBuilder axisBuilder = m_axisBuilders[axisIndex];

                if (axisBuilder is AxisBuilder<TAxis, TArg0, TArg1, TArg2> genericAxisBuilder)
                {
                    if (genericAxisBuilder.AreEqual(arg0, arg1, arg2))
                    {
                        sameIndex = axisIndex;
                        break;
                    }
                }
                else if (axisBuilder is AxisBuilder nonGenericAxisBuilder &&
                        nonGenericAxisBuilder.axisType == typeof(TAxis))
                {
                    const int parametersCacheIndex = parametersCount - 1;
                    object[] parametersCache = m_parametersCache[parametersCacheIndex];
                    parametersCache[0] = arg0;
                    parametersCache[1] = arg1;
                    parametersCache[2] = arg2;

                    if (nonGenericAxisBuilder.AreEqual(parametersCache))
                    {
                        sameIndex = axisIndex;
                        Array.Clear(parametersCache, 0, parametersCount);
                        break;
                    }

                    Array.Clear(parametersCache, 0, parametersCount);
                }
            }

            if (sameIndex >= 0)
            {
                m_actionAxisBindings[^1].Add(sameIndex);
            }
            else
            {
                int count = m_axisBuilders.Count;
                m_actionAxisBindings[^1].Add(count);
                axesLookup.Add(count);
                m_axisBuilders.Add(new AxisBuilder<TAxis, TArg0, TArg1, TArg2>(arg0, arg1, arg2, name));
            }
        }
        public void AddAxis(Type axisType, string name = "", params object[] parameters)
        {
            if (parameters is { Length: > 3 })
            {
                throw new ArgumentException($"Failed to add an axis of type {axisType}. Too many {nameof(parameters)} were passed. The method supports up to 3.");
            }

            if (m_actionAxisBindings.Count == 0)
            {
                throw new InvalidOperationException($"Failed to add an axis of type {axisType}. No action was added before.");
            }

            List<int> axesLookup = m_fastAxesLookup[parameters?.Length ?? 0];
            int sameIndex = -1;

            for (int i = 0, count = axesLookup.Count; i < count; i++)
            {
                int axisIndex = axesLookup[i];
                IAxisBuilder axisBuilder = m_axisBuilders[axisIndex];

                if (axisBuilder.axisType == axisType && axisBuilder.AreEqual(parameters))
                {
                    sameIndex = axisIndex;
                    break;
                }
            }

            if (sameIndex >= 0)
            {
                m_actionAxisBindings[^1].Add(sameIndex);
            }
            else
            {
                int count = m_axisBuilders.Count;
                m_actionAxisBindings[^1].Add(count);
                axesLookup.Add(count);
                m_axisBuilders.Add(new AxisBuilder(axisType, parameters, name));
            }
        }
        #endregion

        #region Makers
        private Axis[] MakeAxes()
        {
            int count = m_axisBuilders.Count;
            Axis[] axes = new Axis[count];

            for (int i = 0; i < count; i++)
            {
                axes[i] = m_axisBuilders[i].Build();
            }

            return axes;
        }

        private Action[] MakeActions()
        {
            int count = m_actionBuilders.Count;
            Action[] actions = new Action[count];

            for (int i = 0; i < count; i++)
            {
                actions[i] = m_actionBuilders[i].Build();
            }

            return actions;
        }

        private int[][] MakeActionAxisBindings()
        {
            int count = m_actionAxisBindings.Count;
            int[][] bindings = new int[count][];

            for (int i = 0; i < count; i++)
            {
                bindings[i] = m_actionAxisBindings[i].ToArray();
            }

            return bindings;
        }
        #endregion
    }
}
