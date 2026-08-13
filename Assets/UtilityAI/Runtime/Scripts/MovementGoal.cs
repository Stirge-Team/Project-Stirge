using Stirge.Combat;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class MovementGoal : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] private float m_scaling = 1f;
        [Tooltip("The length of time this Goal will be performed for until the Enemy attempts to re-evaluate its Movement Goals.")]
        [SerializeField, Min(0f)] private float m_performanceTime;

        public float Evaluate(CombatEntity user)
        {
            float baseScore = EvaluateThis(user);
            return baseScore * m_scaling;
        }
        protected abstract float EvaluateThis(CombatEntity user);
    }
}
