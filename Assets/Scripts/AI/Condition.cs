using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public abstract class Condition
    {
        [SerializeField] private bool m_invertValue = false;

        public bool IsTrue(Agent agent)
        {
            return _IsTrue(agent) == !m_invertValue;
        }

        protected abstract bool _IsTrue(Agent agent);

        // static list of all valid Conditions
        public static readonly System.Type[] ConditionTypes =
        {
            typeof(AirJuggleCondition),
            typeof(DistanceCondition),
            typeof(GroundedCondition),
            typeof(OffGroundCondition),
            typeof(StunnedCondition),
            typeof(TargetInRangeCondition),
            typeof(ArrivedAtTargetCondition),
            typeof(HasTargetCondition)
        };
    }
}
