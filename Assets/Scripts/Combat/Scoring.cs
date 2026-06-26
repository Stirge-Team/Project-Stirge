using UnityEngine;

namespace Stirge.Combat
{
    public static class Scoring
    {
        /// <returns>
        /// <paramref name="input"/> clamped between <paramref name="minValue"/> and <paramref name="maxValue"/>.
        /// </returns>
        public static float GetScore(float input, float minValue, float maxValue)
        {
            return Mathf.Clamp(input, minValue, maxValue);
        }

        /// <returns>
        /// <paramref name="input"/> clamped between <paramref name="minValue"/> and <paramref name="maxValue"/> normalised to a value between 0 and 1.
        /// </returns>
        public static float GetNormalisedScore(float input, float minValue, float maxValue)
        {
            return (GetScore(input, minValue, maxValue) - minValue) / (maxValue - minValue);
        }

        /// <returns>
        /// <paramref name="input"/> normalised to a 0 - 1 scale based on <paramref name="minValue"/> and <paramref name="maxValue"/>.
        /// </returns>
        public static float GetActualNormalisedScore(float input, float minValue, float maxValue)
        {
            return (input - minValue) / (maxValue - minValue);
        }
    }
}
