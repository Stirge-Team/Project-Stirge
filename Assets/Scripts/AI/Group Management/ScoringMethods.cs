using UnityEngine;

namespace Stirge.ScoringMethods
{
    public abstract class ScoringMethod
    {
        public abstract float ScoreFunction();
    }

    public class DistanceScore : ScoringMethod
    {
        private Transform m_transformA;
        private Transform m_transformB;

        public DistanceScore(Transform transformA, Transform transformB)
        {
            m_transformA = transformA;
            m_transformB = transformB;
        }
        public override float ScoreFunction()
        {
            return Vector3.Distance(m_transformB.position, m_transformA.position);
        }
    }

    public class SpeedScore : ScoringMethod
    {
        private Rigidbody m_rb;

        public SpeedScore(Rigidbody rb)
        {
            m_rb = rb;
        }

        public override float ScoreFunction()
        {
            return m_rb.linearVelocity.sqrMagnitude;
        }
    }
}
