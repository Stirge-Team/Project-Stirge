using System;
using UnityEngine;

namespace Stirge.Tools
{
    [Serializable]
    public struct AnimationCurve3D
    {
        [SerializeField] private AnimationCurve m_x;
        [SerializeField] private AnimationCurve m_y;
        [SerializeField] private AnimationCurve m_z;

        public readonly Vector3 Evaluate(float time)
        {
            return new(m_x.Evaluate(time), m_y.Evaluate(time), m_z.Evaluate(time));
        }
    }
}
