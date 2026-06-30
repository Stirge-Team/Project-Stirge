using UnityEngine;
using System;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI.Serialization
{
    using Builders;
    using Core;

    [CreateAssetMenu(menuName = "Stirge/Utility AI/Serialized Actor", fileName = "New Serialized Actor", order = 449)]
    public sealed class SerializedActor : SerializedActor_Base
    {
        [SerializeField] private SerializedAction_Base[] m_serializedActions;
        [SerializeField] private SerializedAxis_Base[] m_serializedAxes;
        [SerializeField] private AxisIndices[] m_axisIndices;

        private ActorBuilder m_builder;

        public override Actor CreateActor(Blackboard blackboard)
        {
            Deserialize();
            Actor actor = m_builder.Build(blackboard);

            return actor;
        }

        private void Deserialize()
        {
            if (m_builder != null)
            {
                return;
            }

            m_builder = new ActorBuilder();

            for (int actionIndex = 0, actionCount = m_serializedActions.Length; actionIndex < actionCount; ++actionIndex)
            {
                SerializedAction_Base serializedActionBase = m_serializedActions[actionIndex];
                serializedActionBase.AddAction(m_builder);

                int[] axisIndices = m_axisIndices[actionIndex].axes;

                for (int axisIndex = 0, axisCount = axisIndices.Length; axisIndex < axisCount; ++axisIndex)
                {
                    SerializedAxis_Base serializedAxisBase = m_serializedAxes[axisIndices[axisIndex]];
                    serializedAxisBase.AddAxis(m_builder);
                }
            }
        }

        private void OnValidate()
        {
            m_builder = null;

            int serializedActionCount = m_serializedActions.Length;

            Array.Resize(ref m_axisIndices, serializedActionCount);

            for (int actionIndex = 0, actionCount = m_axisIndices.Length;
                actionIndex < actionCount;
                ++actionIndex)
            {
                int[] axisIndices = m_axisIndices[actionIndex].axes;

                for (int axisIndex = 0, axisCount = axisIndices.Length; axisIndex < axisCount; ++axisIndex)
                {
                    int axis = axisIndices[axisIndex];

                    if (axis < 0 || axis >= serializedActionCount)
                    {
                        RemoveElement(ref axisIndices, axisIndex--);
                        axisCount = axisIndices.Length;
                    }
                }
            }
        }

        private static void RemoveElement(ref int[] array, int element)
        {
            for (int i = element, count = array.Length - 1; i < count; ++i)
            {
                array[i] = array[i + 1];
            }

            Array.Resize(ref array, array.Length - 1);
        }

        [Serializable]
        private struct AxisIndices
        {
            [SerializeField] private int[] m_axes;

            public int[] axes => m_axes;
        }
    }
}
