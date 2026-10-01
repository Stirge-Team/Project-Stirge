using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.EditorTools
{
    [InitializeOnLoad]
    public class SerializedConditionTypesCollection
    {
        private static readonly Type s_actionConditionBase = typeof(Condition<Action>);
        private static readonly Type s_movementGoalConditionBase = typeof(Condition<MovementGoal>);
        private static readonly Type s_statusConditionBase = typeof(Condition<Status>);

        private static readonly Type[] s_serializedConditionTypes;
        private static readonly Type[] s_conditionTypes;

        private static readonly Type[] s_serializedActionConditionTypes;
        private static readonly Type[] s_actionConditionTypes;

        private static readonly Type[] s_serializedMovementGoalConditionTypes;
        private static readonly Type[] s_movementGoalConditionTypes;

        private static readonly Type[] s_serializedStatusConditionTypes;
        private static readonly Type[] s_statusConditionTypes;

        static SerializedConditionTypesCollection()
        {
            // Base
            List<Type> serializedConditionTypesList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     where !domainAssembly.IsDynamic
                                     from assemblyType in domainAssembly.GetExportedTypes()
                                     where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                        && assemblyType.IsSubclassOf(typeof(SerializedCondition_Base))
                                     select assemblyType)
                .ToList();
            serializedConditionTypesList.Remove(typeof(SerializedGenericCondition));
            serializedConditionTypesList.Insert(0, typeof(SerializedGenericCondition));

            s_serializedConditionTypes = serializedConditionTypesList.ToArray();
            int count = s_serializedConditionTypes.Length;
            s_conditionTypes = new Type[count];

            List<Type> serializedActionConditionTypes = new();
            List<Type> actionConditionTypes = new();

            List<Type> serializedMovementGoalConditionTypes = new();
            List<Type> movementGoalConditionTypes = new();

            List<Type> serializedStatusConditionTypes = new();
            List<Type> statusConditionTypes = new();

            for (int i = 0; i < count; i++)
            {
                Type serializedConditionType = s_serializedConditionTypes[i];
                var tempSerializedCondition = (SerializedCondition_Base)ScriptableObject.CreateInstance(serializedConditionType);
                Type conditionType = tempSerializedCondition.ConditionType;

                // Action
                if (conditionType.IsSubclassOf(s_actionConditionBase))
                {
                    serializedActionConditionTypes.Add(serializedConditionType);
                    actionConditionTypes.Add(conditionType);
                }
                // Movement Goal
                else if (conditionType.IsSubclassOf(s_movementGoalConditionBase))
                {
                    serializedMovementGoalConditionTypes.Add(serializedConditionType);
                    movementGoalConditionTypes.Add(conditionType);
                }
                // Status
                else if (conditionType.IsSubclassOf(s_statusConditionBase))
                {
                    serializedStatusConditionTypes.Add(serializedConditionType);
                    statusConditionTypes.Add(conditionType);
                }
                // All / IScorable
                else // if (conditionType.IsSubclassOf(typeof(Condition<IScorable>)) || conditionType.IsSubclassOf(typeof(GenericCondition_Base)))
                {
                    serializedActionConditionTypes.Add(serializedConditionType);
                    actionConditionTypes.Add(conditionType);

                    serializedMovementGoalConditionTypes.Add(serializedConditionType);
                    movementGoalConditionTypes.Add(conditionType);

                    serializedStatusConditionTypes.Add(serializedConditionType);
                    statusConditionTypes.Add(conditionType);
                }

                s_conditionTypes[i] = conditionType;
                Object.DestroyImmediate(tempSerializedCondition);
            }

            // Action
            s_serializedActionConditionTypes = serializedActionConditionTypes.ToArray();
            s_actionConditionTypes = actionConditionTypes.ToArray();

            // Movement Goal
            s_serializedMovementGoalConditionTypes = serializedMovementGoalConditionTypes.ToArray();
            s_movementGoalConditionTypes = movementGoalConditionTypes.ToArray();
            
            // Status
            s_serializedStatusConditionTypes = serializedStatusConditionTypes.ToArray();
            s_statusConditionTypes = statusConditionTypes.ToArray();
        }

        public static IReadOnlyList<Type> GetScoringMethodTypes<TScorable>() where TScorable : IScorable
        {
            if (typeof(TScorable) == typeof(Action))
            {
                return s_actionConditionTypes;
            }
            else if (typeof(TScorable) == typeof(MovementGoal))
            {
                return s_movementGoalConditionTypes;
            }
            else if (typeof(TScorable) == typeof(Status))
            {
                return s_statusConditionTypes;
            }
            else
            {
                return s_conditionTypes;
            }
        }

        public static Type GetSerializedConditionType<TScorable>(Type conditionType) where TScorable : IScorable
        {
            if (typeof(TScorable) == typeof(Action))
            {
                int index = Array.IndexOf(s_actionConditionTypes, conditionType);
                return index >= 0 ? s_serializedActionConditionTypes[index] : null;
            }
            else if (typeof(TScorable) == typeof(MovementGoal))
            {
                int index = Array.IndexOf(s_movementGoalConditionTypes, conditionType);
                return index >= 0 ? s_serializedMovementGoalConditionTypes[index] : null;
            }
            else if (typeof(TScorable) == typeof(Status))
            {
                int index = Array.IndexOf(s_statusConditionTypes, conditionType);
                return index >= 0 ? s_serializedStatusConditionTypes[index] : null;
            }
            else
            {
                int index = Array.IndexOf(s_conditionTypes, conditionType);
                return index >= 0 ? s_serializedConditionTypes[index] : null;
            }
        }
    }
}
