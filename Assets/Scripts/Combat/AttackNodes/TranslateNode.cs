using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    [System.Serializable]
    public class TranslateNode : AttackNode, ISetupable<Vector3, bool, RandomFloatField>
    {
        [SerializeField] private Vector3 m_translation;
        [SerializeField] private bool m_isLocalTranslation = true;
        [SerializeField] private RandomFloatField m_time;

        public Vector3 Translation => m_translation;
        public bool IsLocalTranslation => m_isLocalTranslation;
        public float Time => m_time.Value;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            activeNodes.Add(this);
        }

        public void Setup(Vector3 translation, bool isLocalTranslation, RandomFloatField time)
        {
            m_translation = translation;
            m_isLocalTranslation = isLocalTranslation;
            m_time = time;
        }
    }
}
