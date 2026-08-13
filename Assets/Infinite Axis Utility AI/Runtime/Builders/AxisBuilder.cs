using System;
using UnityEngine;

namespace Stirge.UtilityAI.Builders
{
    using Core;
    using System.Collections.Generic;
    using Stirge.Serialization;

    public sealed class AxisBuilder<TAxis> : IAxisBuilder where TAxis : Axis, INotSetupable, new()
    {
        private readonly string m_name;

        public AxisBuilder(string name)
        {
            m_name = name;
        }

        public Type axisType => typeof(TAxis);

        public Axis Build()
        {
            Axis axis = Axis.Create<TAxis>();
            axis.name = m_name;
            return axis;
        }

        public bool AreEqual(object[] parameters)
        {
            return parameters == null || parameters.Length == 0;
        }
    }
    
    public sealed class AxisBuilder<TAxis, TArg> : IAxisBuilder where TAxis : Axis, ISetupable<TArg>, new()
    {
        private readonly TArg m_arg;

        private readonly string m_name;

        public AxisBuilder(TArg arg, string name)
        {
            m_arg = arg;

            m_name = name;
        }

        public Type axisType => typeof(TAxis);

        public Axis Build()
        {
            TAxis axis = Axis.Create<TAxis, TArg>(m_arg);
            axis.name = m_name;
            return axis;
        }

        public bool AreEqual(TArg arg)
        {
            return EqualityComparer<TArg>.Default.Equals(m_arg, arg);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg == null ? parameters[0] == null : parameters[0] is TArg parameter && EqualityComparer<TArg>.Default.Equals(m_arg, parameter);
        }
    }

    public sealed class AxisBuilder<TAxis, TArg0, TArg1> : IAxisBuilder where TAxis : Axis, ISetupable<TArg0, TArg1>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;

        private readonly string m_name;

        public AxisBuilder(TArg0 arg0, TArg1 arg1, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;

            m_name = name;
        }

        public Type axisType => typeof(TAxis);

        public Axis Build()
        {
            TAxis axis = Axis.Create<TAxis, TArg0, TArg1>(m_arg0, m_arg1);
            axis.name = m_name;
            return axis;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                   EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1);
        }
    }

    public sealed class AxisBuilder<TAxis, TArg0, TArg1, TArg2> : IAxisBuilder where TAxis : Axis, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;

        private readonly string m_name;

        public AxisBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;

            m_name = name;
        }

        public Type axisType => typeof(TAxis);

        public Axis Build()
        {
            TAxis axis = Axis.Create<TAxis, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2);
            axis.name = m_name;
            return axis;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1) &&
                EqualityComparer<TArg2>.Default.Equals(m_arg2, arg2);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1) &&
                   m_arg2 == null ? parameters[2] == null : parameters[2] is TArg2 parameter2 && EqualityComparer<TArg2>.Default.Equals(m_arg2, parameter2);
        }
    }

    public sealed class AxisBuilder<TAxis, TArg0, TArg1, TArg2, TArg3> : IAxisBuilder where TAxis : Axis, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;
        private readonly TArg3 m_arg3;

        private readonly string m_name;

        public AxisBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_arg3 = arg3;

            m_name = name;
        }

        public Type axisType => typeof(TAxis);

        public Axis Build()
        {
            TAxis axis = Axis.Create<TAxis, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3);
            axis.name = m_name;
            return axis;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1) &&
                EqualityComparer<TArg2>.Default.Equals(m_arg2, arg2) &&
                EqualityComparer<TArg3>.Default.Equals(m_arg3, arg3);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1) &&
                   m_arg2 == null ? parameters[2] == null : parameters[2] is TArg2 parameter2 && EqualityComparer<TArg2>.Default.Equals(m_arg2, parameter2) &&
                   m_arg3 == null ? parameters[3] == null : parameters[3] is TArg3 parameter3 && EqualityComparer<TArg3>.Default.Equals(m_arg3, parameter3);
        }
    }

    public sealed class AxisBuilder<TAxis, TArg0, TArg1, TArg2, TArg3, TArg4> : IAxisBuilder where TAxis : Axis, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;
        private readonly TArg3 m_arg3;
        private readonly TArg4 m_arg4;

        private readonly string m_name;

        public AxisBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_arg3 = arg3;
            m_arg4 = arg4;

            m_name = name;
        }

        public Type axisType => typeof(TAxis);

        public Axis Build()
        {
            TAxis axis = Axis.Create<TAxis, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4);
            axis.name = m_name;
            return axis;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1) &&
                EqualityComparer<TArg2>.Default.Equals(m_arg2, arg2) &&
                EqualityComparer<TArg3>.Default.Equals(m_arg3, arg3) &&
                EqualityComparer<TArg4>.Default.Equals(m_arg4, arg4);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1) &&
                   m_arg2 == null ? parameters[2] == null : parameters[2] is TArg2 parameter2 && EqualityComparer<TArg2>.Default.Equals(m_arg2, parameter2) &&
                   m_arg3 == null ? parameters[3] == null : parameters[3] is TArg3 parameter3 && EqualityComparer<TArg3>.Default.Equals(m_arg3, parameter3) &&
                   m_arg4 == null ? parameters[4] == null : parameters[4] is TArg4 parameter4 && EqualityComparer<TArg4>.Default.Equals(m_arg4, parameter4);
        }
    }

    public sealed class AxisBuilder : IAxisBuilder
    {
        [SerializeField] private string m_name;
        [SerializeField] private Type m_axisType;
        [SerializeField] private object[] m_parameters;
        public AxisBuilder(Type axisType, object[] parameters = null, string name = "")
        {
            m_axisType = axisType;
            m_parameters = parameters;
            m_name = name;
        }

        public Type axisType => m_axisType;

        public bool AreEqual(object[] parameters)
        {
            if (m_parameters == null)
            {
                return parameters == null;
            }

            if (parameters == null)
            {
                return false;
            }

            if (m_parameters.Length != parameters.Length)
            {
                return false;
            }

            for (int i = 0, count = m_parameters.Length; i < count; ++i)
            {
                if (m_parameters[i] == null)
                {
                    if (parameters[i] != null)
                    {
                        return false;
                    }
                }
                else if (!m_parameters[i].Equals(parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public Axis Build()
        {
            Axis axis = m_parameters != null
                ? Axis.Create(m_axisType, m_parameters)
                : Axis.Create(m_axisType);
            axis.name = m_name;
            return axis;
        }
    }
}
