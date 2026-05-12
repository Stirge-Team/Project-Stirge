using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class AnimationNode : AttackNode
    {
        [SerializeField] private string m_animationStateName;
        [SerializeField] private AnimationClip m_animationClip;
        [SerializeField] RandomFloatField m_speed;
        [SerializeField] private bool m_hasRootMotion;

        public string AnimationStateName => m_animationStateName;
        public AnimationClip AnimationClip => m_animationClip;
        public float Speed => m_speed.Value;
        public bool HasRootMotion => m_hasRootMotion;
        public float Time => m_animationClip.length / Speed;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_speed.DetermineValue();

            // ensure name is not empty
            if (m_animationStateName == string.Empty)
                m_animationStateName = m_animationClip.name;

            activeNodes.Add(this);
        }
    }
}
