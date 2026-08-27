using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface IScalable
    {
        public float ScoreScaling { get; }

        public void SetScoreScaling(float newScaling);
    }
}
