using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class Condition
    {   
        [SerializeField] private int m_typeIndex = 0;
        [SerializeField] private bool m_invertValue = false;

        public bool IsTrue(Agent agent)
        {
            return _IsTrue(agent) == !m_invertValue;
        }

        protected virtual bool _IsTrue(Agent agent)
        {
            return true;
        }
    }
}
