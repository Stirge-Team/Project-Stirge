using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class AnimationNode : AttackNode
    {
        [SerializeField] protected AnimationClip m_animation;
        [SerializeField] RandomFloatField m_speed;
        [SerializeField] private bool m_hasRootMotion;

        public AnimationClip Animation => m_animation;
        public float Speed => m_speed.Value;
        public bool HasRootMotion => m_hasRootMotion;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_speed.DetermineValue();
            activeNodes.Add(this);
        }

        public override float EvaluateTime()
        {
            return m_animation.length * Speed;
        }
    }
}
