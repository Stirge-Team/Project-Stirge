using System;
using Stirge.Combat;
using UnityEngine;

namespace Stirge.ScoringMethods
{
    public abstract class ScoringMethod
    {
        protected float m_minValue;
        protected float m_maxValue;
        protected float m_valueRange => m_maxValue - m_minValue;
        protected bool m_doNormalise = false;
        public abstract float ScoreFunction();
        public virtual ScoringMethod SetMinMax(float min, float max)
        {
            if (min >= max)
            {
                Debug.LogWarning("Minimum is greater or equal then the maximum... you are a fool!");
                return this;
            }
            m_minValue = min;
            m_maxValue = max;
            m_doNormalise = true;
            return this;
        }
        protected float NormaliseScore(float score)
        {
            score = Mathf.Clamp(score, m_minValue, m_maxValue); //clamp the score between the min and max values
            if (score == m_minValue || score == m_maxValue) //if the score is at either bound
            {
                return score == m_minValue ? 0 : 1; //if its at the min, return 0, otherwise its at the max so return 1
            }

            return (score - m_minValue) / m_valueRange; //do the normalision
        }
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
        /// <summary>
        /// Returns the distance between the 2 given transforms.
        /// </summary>
        /// <returns></returns>
        public override float ScoreFunction()
        {
            float distance = Vector3.Distance(m_transformB.position, m_transformA.position);
            if (m_doNormalise) return NormaliseScore(distance);
            return distance;
        }
    }

    public class SpeedScore : ScoringMethod
    {
        private Rigidbody m_rb;

        public SpeedScore(Rigidbody rb)
        {
            m_rb = rb;
        }

        /// <summary>
        /// Returns the squared magnitude of the linear velocity of the given rigidbody.
        /// </summary>
        /// <returns></returns>
        public override float ScoreFunction()
        {
            float velocity = m_rb.linearVelocity.sqrMagnitude;
            if (m_doNormalise) return NormaliseScore(velocity);
            return velocity;
        }
    }
    public class HealthScore : ScoringMethod
    {
        private CombatEntity m_entity;
        public HealthScore(CombatEntity entity)
        {
            m_entity = entity;
        }
        /// <summary>
        /// Returns this entity's percentage of health remaining
        /// </summary>
        /// <returns></returns>
        public override float ScoreFunction()
        {
            float health = m_entity.Health._healthPercent;
            if (m_doNormalise) return NormaliseScore(health);
            return health;
        }
    }
    public class EnemyCountScore : ScoringMethod
    {
        //this will need to be change to something that
        private Enemy.Enemy[] m_groupOfEnemies;
        public EnemyCountScore()
        {
            m_groupOfEnemies = MonoBehaviour.FindObjectsByType<Enemy.Enemy>(FindObjectsSortMode.None);
            if (m_groupOfEnemies.Length == 0)
            {
                Debug.LogWarning("There are no enemies in the scene, so this scoring method will return 0 when scored!");
            }
        }
        public EnemyCountScore(Enemy.Enemy[] enemies)
        {
            m_groupOfEnemies = enemies;
        }

        public override float ScoreFunction()
        {
            float enemyCount = 0;
            foreach (var enemy in m_groupOfEnemies)
                if (enemy != null) enemyCount++;

            if (m_doNormalise) return NormaliseScore(enemyCount);
            return enemyCount;
        }
    }
    public class OutsideRange : ScoringMethod
    {
        private Transform m_originTransform;
        private Transform m_targetTransform;
        /// <summary>
        /// This function will automatically apply a minimum and maximum value, and will normalise the result when scored.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="minRange"></param>
        public OutsideRange(Transform origin, Transform target, float minRange)
        {
            m_originTransform = origin;
            m_targetTransform = target;
            SetMinMax(minRange, Int32.MaxValue / 1000);
        }
        /// <summary>
        /// Will return a normalised value based on how close the target transform is from the origin transform, with 1 being as close as possible WITHOUT being within the range itself.
        /// </summary>
        /// <returns></returns>
        public override float ScoreFunction()
        {
            float distance = Vector3.Distance(m_originTransform.position, m_targetTransform.position);
            if(distance < m_minValue) return 0;
            return 1 - NormaliseScore(distance);
        }
    }
}
