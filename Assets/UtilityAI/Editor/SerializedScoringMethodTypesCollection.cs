using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.EditorTools
{
    [InitializeOnLoad]
    public class SerializedScoringMethodTypesCollection
    {
        private static readonly Type s_actionScoringMethodBase = typeof(ScoringMethod<Action>);
        private static readonly Type s_movementGoalScoringMethodBase = typeof(ScoringMethod<MovementGoal>);
        private static readonly Type s_statusScoringMethodBase = typeof(ScoringMethod<Status>);

        private static readonly Type[] s_serializedScoringMethodTypes;
        private static readonly Type[] s_scoringMethodTypes;

        private static readonly Type[] s_serializedActionScoringMethodTypes;
        private static readonly Type[] s_actionScoringMethodTypes;

        private static readonly Type[] s_serializedMovementGoalScoringMethodTypes;
        private static readonly Type[] s_movementGoalScoringMethodTypes;

        private static readonly Type[] s_serializedStatusScoringMethodTypes;
        private static readonly Type[] s_statusScoringMethodTypes;

        static SerializedScoringMethodTypesCollection()
        {
            // Base
            s_serializedScoringMethodTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     where !domainAssembly.IsDynamic
                                     from assemblyType in domainAssembly.GetExportedTypes()
                                     where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                        && assemblyType.IsSubclassOf(typeof(SerializedScoringMethod_Base))
                                     select assemblyType)
                .ToArray();

            int count = s_serializedScoringMethodTypes.Length;
            s_scoringMethodTypes = new Type[count];

            List<Type> serializedActionScoringMethodTypes = new();
            List<Type> actionScoringMethodTypes = new();

            List<Type> serializedMovementGoalScoringMethodTypes = new();
            List<Type> movementGoalScoringMethodTypes = new();

            List<Type> serializedStatusScoringMethodTypes = new();
            List<Type> statusScoringMethodTypes = new();

            for (int i = 0; i < count; i++)
            {
                Type serializedScoringMethodType = s_serializedScoringMethodTypes[i];
                var tempSerializedScoringMethod = (SerializedScoringMethod_Base)ScriptableObject.CreateInstance(serializedScoringMethodType);
                Type scoringMethodType = tempSerializedScoringMethod.ScoringMethodType;

                // Action
                if (scoringMethodType.IsSubclassOf(s_actionScoringMethodBase))
                {
                    serializedActionScoringMethodTypes.Add(serializedScoringMethodType);
                    actionScoringMethodTypes.Add(scoringMethodType);
                }
                // Movement Goal
                else if (scoringMethodType.IsSubclassOf(s_movementGoalScoringMethodBase))
                {
                    serializedMovementGoalScoringMethodTypes.Add(serializedScoringMethodType);
                    movementGoalScoringMethodTypes.Add(scoringMethodType);
                }
                // Status
                else if (scoringMethodType.IsSubclassOf(s_statusScoringMethodBase))
                {
                    serializedStatusScoringMethodTypes.Add(serializedScoringMethodType);
                    statusScoringMethodTypes.Add(scoringMethodType);
                }
                // All / IScorable
                else // if (scoringMethodType.IsSubclassOf(typeof(ScoringMethod<IScorable>)))
                {
                    serializedActionScoringMethodTypes.Add(serializedScoringMethodType);
                    actionScoringMethodTypes.Add(scoringMethodType);

                    serializedMovementGoalScoringMethodTypes.Add(serializedScoringMethodType);
                    movementGoalScoringMethodTypes.Add(scoringMethodType);

                    serializedStatusScoringMethodTypes.Add(serializedScoringMethodType);
                    statusScoringMethodTypes.Add(scoringMethodType);
                }

                s_scoringMethodTypes[i] = scoringMethodType;
                Object.DestroyImmediate(tempSerializedScoringMethod);
            }

            // Action
            s_serializedActionScoringMethodTypes = serializedActionScoringMethodTypes.ToArray();
            s_actionScoringMethodTypes = actionScoringMethodTypes.ToArray();

            // Movement Goal
            s_serializedMovementGoalScoringMethodTypes = serializedMovementGoalScoringMethodTypes.ToArray();
            s_movementGoalScoringMethodTypes = movementGoalScoringMethodTypes.ToArray();
            
            // Status
            s_serializedStatusScoringMethodTypes = serializedStatusScoringMethodTypes.ToArray();
            s_statusScoringMethodTypes = statusScoringMethodTypes.ToArray();
        }

        public static IReadOnlyList<Type> GetScoringMethodTypes<TScorable>() where TScorable : IScorable
        {
            if (typeof(TScorable) == typeof(Action))
            {
                return s_actionScoringMethodTypes;
            }
            else if (typeof(TScorable) == typeof(MovementGoal))
            {
                return s_movementGoalScoringMethodTypes;
            }
            else if (typeof(TScorable) == typeof(Status))
            {
                return s_statusScoringMethodTypes;
            }
            else
            {
                return s_scoringMethodTypes;
            }
        }

        public static Type GetSerializedScoringMethodType<TScorable>(Type scoringMethodType) where TScorable : IScorable
        {
            if (typeof(TScorable) == typeof(Action))
            {
                int index = Array.IndexOf(s_actionScoringMethodTypes, scoringMethodType);
                return index >= 0 ? s_serializedActionScoringMethodTypes[index] : null;
            }
            else if (typeof(TScorable) == typeof(MovementGoal))
            {
                int index = Array.IndexOf(s_movementGoalScoringMethodTypes, scoringMethodType);
                return index >= 0 ? s_serializedMovementGoalScoringMethodTypes[index] : null;
            }
            else if (typeof(TScorable) == typeof(Status))
            {
                int index = Array.IndexOf(s_statusScoringMethodTypes, scoringMethodType);
                return index >= 0 ? s_serializedStatusScoringMethodTypes[index] : null;
            }
            else
            {
                int index = Array.IndexOf(s_scoringMethodTypes, scoringMethodType);
                return index >= 0 ? s_serializedScoringMethodTypes[index] : null;
            }
        }
    }
}
