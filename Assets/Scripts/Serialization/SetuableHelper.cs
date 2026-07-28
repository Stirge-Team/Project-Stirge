using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

namespace Stirge.Serialization
{
    public static class SetuableHelper
    {
        public static void CreateSetup(object obj, object[] parameters)
        {
            Type type = obj.GetType();
            Type[] interfaceTypes = type.GetInterfaces();

            int parametersCount = parameters.Length;
            Type[] parameterTypes = new Type[parametersCount];

            for (int i = 0; i < parametersCount; i++)
            {
                object param = parameters[i];
                parameterTypes[i] = param?.GetType();
            }

            Type baseSetupableType = parametersCount switch
            {
                1 => typeof(ISetupable<>),
                2 => typeof(ISetupable<,>),
                3 => typeof(ISetupable<,,>),
                4 => typeof(ISetupable<,,,>),
                5 => typeof(ISetupable<,,,,>),
                _ => throw new ArgumentException($"Failed to setup an object of type {type}. Too many parameters are passed. It supports up to 5 parameters.")
            };

            for (int i = 0, iCount = interfaceTypes.Length; i < iCount; ++i)
            {
                Type interfaceType = interfaceTypes[i];

                // TODO Support derivation
                if (!interfaceType.IsGenericType || interfaceType.GetGenericTypeDefinition() != baseSetupableType)
                {
                    continue;
                }

                Type[] interfaceParameters = interfaceType.GetGenericArguments();
                bool goodInterface = true;

                for (int j = 0, jCount = interfaceParameters.Length; j < jCount & goodInterface; ++j)
                {
                    goodInterface = parameterTypes[j] == null
                        ? interfaceParameters[j].IsClass
                        : interfaceParameters[j].IsAssignableFrom(parameterTypes[j]);
                }

                if (!goodInterface)
                {
                    continue;
                }

                Profiler.BeginSample("Setup");
                interfaceType.InvokeMember("Setup", BindingFlags.InvokeMethod, null, obj, parameters);
                Profiler.EndSample();

                return;
            }

            throw new ArgumentException($"Failed to setup an object of type {type}. It doesn't have an appropriate setup method for passed arguments.");
        }
    }
}
