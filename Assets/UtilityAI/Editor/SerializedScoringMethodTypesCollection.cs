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
        private static readonly Type[] s_serializedScorableScoringMethodTypes;
        private static readonly Type[] s_scorableScoringMethodTypes;

        private static readonly Type[] s_serializedActionScoringMethodTypes;
        private static readonly Type[] s_actionScoringMethodTypes;

        private static readonly Type[] s_serializedMovementGoalScoringMethodTypes;
        private static readonly Type[] s_movementGoalScoringMethodTypes;

        private static readonly Type[] s_serializedStatusScoringMethodTypes;
        private static readonly Type[] s_statusScoringMethodTypes;

        static SerializedScoringMethodTypesCollection()
        {
            // Base / IScorable
            s_serializedScorableScoringMethodTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     where !domainAssembly.IsDynamic
                                     from assemblyType in domainAssembly.GetExportedTypes()
                                     where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                        && assemblyType.IsSubclassOf(typeof(SerializedScoringMethod_Base<IScorable>))
                                     select assemblyType)
                .ToArray();

            int countScorable = s_serializedScorableScoringMethodTypes.Length;
            s_scorableScoringMethodTypes = new Type[countScorable];

            for (int i = 0; i < countScorable; i++)
            {
                var tempSerializedTable = (SerializedScoringMethod_Base<IScorable>)ScriptableObject.CreateInstance(s_serializedScorableScoringMethodTypes[i]);
                s_scorableScoringMethodTypes[i] = tempSerializedTable.scoringMethodType;
                Object.DestroyImmediate(tempSerializedTable);
            }

            // Action
            Type[] serializedActionScoringMethodTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                       where !domainAssembly.IsDynamic
                                                       from assemblyType in domainAssembly.GetExportedTypes()
                                                       where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                                          && assemblyType.IsSubclassOf(typeof(SerializedScoringMethod_Base<Action>))
                                                       select assemblyType)
                .ToArray();

            int countAction = serializedActionScoringMethodTypes.Length;
            Type[] actionScoringMethods = new Type[countAction];

            for (int i = 0; i < countAction; i++)
            {
                var tempSerializedTable = (SerializedScoringMethod_Base<Action>)ScriptableObject.CreateInstance(serializedActionScoringMethodTypes[i]);
                actionScoringMethods[i] = tempSerializedTable.scoringMethodType;
                Object.DestroyImmediate(tempSerializedTable);
            }
            s_serializedActionScoringMethodTypes = s_serializedScorableScoringMethodTypes.Union(serializedActionScoringMethodTypes).ToArray();
            s_actionScoringMethodTypes = s_scorableScoringMethodTypes.Union(actionScoringMethods).ToArray();

            // Movement Goal
            Type[] serializedMovementGoalScoringMethodTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                         where !domainAssembly.IsDynamic
                                                         from assemblyType in domainAssembly.GetExportedTypes()
                                                         where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                                            && assemblyType.IsSubclassOf(typeof(SerializedScoringMethod_Base<MovementGoal>))
                                                         select assemblyType)
                .ToArray();

            int countMovementGoal = serializedMovementGoalScoringMethodTypes.Length;
            Type[] movementGoalScoringMethods = new Type[countMovementGoal];

            for (int i = 0; i < countMovementGoal; i++)
            {
                var tempSerializedTable = (SerializedScoringMethod_Base<MovementGoal>)ScriptableObject.CreateInstance(serializedMovementGoalScoringMethodTypes[i]);
                movementGoalScoringMethods[i] = tempSerializedTable.scoringMethodType;
                Object.DestroyImmediate(tempSerializedTable);
            }
            s_serializedMovementGoalScoringMethodTypes = s_serializedScorableScoringMethodTypes.Union(serializedMovementGoalScoringMethodTypes).ToArray();
            s_movementGoalScoringMethodTypes = s_scorableScoringMethodTypes.Union(movementGoalScoringMethods).ToArray();

            // Status
            Type[] serializedStatusScoringMethodTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                               where !domainAssembly.IsDynamic
                                                               from assemblyType in domainAssembly.GetExportedTypes()
                                                               where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                                                  && assemblyType.IsSubclassOf(typeof(SerializedScoringMethod_Base<Status>))
                                                               select assemblyType)
                .ToArray();

            int countStatus = serializedStatusScoringMethodTypes.Length;
            Type[] statusScoringMethods = new Type[countStatus];

            for (int i = 0; i < countStatus; i++)
            {
                var tempSerializedTable = (SerializedScoringMethod_Base<Status>)ScriptableObject.CreateInstance(serializedStatusScoringMethodTypes[i]);
                statusScoringMethods[i] = tempSerializedTable.scoringMethodType;
                Object.DestroyImmediate(tempSerializedTable);
            }
            s_serializedStatusScoringMethodTypes = s_serializedScorableScoringMethodTypes.Union(serializedStatusScoringMethodTypes).ToArray();
            s_statusScoringMethodTypes = s_scorableScoringMethodTypes.Union(statusScoringMethods).ToArray();
        }

        public static IReadOnlyList<Type> scorableScoringMethodTypes => s_scorableScoringMethodTypes;
        public static IReadOnlyList<Type> actionScoringMethodTypes => s_actionScoringMethodTypes;
        public static IReadOnlyList<Type> movementGoalScoringMethodTypes => s_movementGoalScoringMethodTypes;
        public static IReadOnlyList<Type> statusScoringMethodTypes => s_statusScoringMethodTypes;

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
                int index = Array.IndexOf(s_scorableScoringMethodTypes, scoringMethodType);
                return index >= 0 ? s_serializedScorableScoringMethodTypes[index] : null;
            }
        }
    }
}
