using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class ScoringMethod<TParam>
    {
        protected float m_scoreScaling;

        public float Evaluate(TParam param)
        {
            float score = EvaluateInternal(param);
            return score * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(TParam param1);
    }

    public abstract class ScoringMethod<TParam0, TParam1>
    {
        protected float m_scoreScaling;

        public float Evaluate(TParam0 param0, TParam1 param1)
        {
            float score = EvaluateInternal(param0, param1);
            return score * m_scoreScaling;
        }

        protected abstract float EvaluateInternal(TParam0 param0, TParam1 param1);
    }
}
