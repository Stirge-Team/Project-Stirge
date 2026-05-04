using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class TranslateNode : AttackNode
    {
        [SerializeField] private bool m_isLocalTranslation = true;
        [SerializeField] private Vector3 m_translation;
        [SerializeField, Min(0)] private float m_time;

        public bool IsLocalTranslation => m_isLocalTranslation;
        public Vector3 Translation => m_translation;
        public float Time => m_time;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            activeNodes.Add(this);
        }

        public override float EvaluateTime()
        {
            return m_time;
        }
    }
}
