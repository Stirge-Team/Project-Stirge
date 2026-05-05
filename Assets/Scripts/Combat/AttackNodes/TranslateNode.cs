using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class TranslateNode : AttackNode
    {
        [SerializeField] private Vector3 m_translation;
        [SerializeField] private bool m_isLocalTranslation = true;
        [SerializeField, Min(0)] private RandomFloatField m_time;

        public Vector3 Translation => m_translation;
        public bool IsLocalTranslation => m_isLocalTranslation;
        public float Time => m_time.Value;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            activeNodes.Add(this);
        }

        public override float EvaluateTime()
        {
            return Time;
        }
    }
}
