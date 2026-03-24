using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class Condition
    {
#if UNITY_EDITOR
        [SerializeField] private int m_typeIndex = 0;
#endif
        [SerializeField] private bool m_invertValue = false;

        public bool IsTrue(Agent agent)
        {
            return _IsTrue(agent) == !m_invertValue;
        }

        protected virtual bool _IsTrue(Agent agent)
        {
            return true;
        }

        // static list of all valid Conditions
        public static readonly System.Type[] ConditionTypes =
        {
            typeof(Condition),
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
