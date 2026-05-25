using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class Axis
    {
        [SerializeField] protected AnimationCurve m_curve;

        public abstract float ComputeScore();

        protected virtual void OnInitialise() { }

        protected virtual void OnDispose() { }

        internal void Initialise()
        {
            OnInitialise();
        }

        internal void Dispose()
        {
            OnDispose();
        }
    }
}
