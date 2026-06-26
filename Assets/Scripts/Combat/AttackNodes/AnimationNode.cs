using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    [System.Serializable]
    public class AnimationNode : AttackNode, ISetupable<string, AnimationClip, RandomFloatField, bool>
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

        public void Setup(string animationStateName, AnimationClip animationClip, RandomFloatField speed, bool hasRootMotion)
        {
            m_animationStateName = animationStateName;
            m_animationClip = animationClip;
            m_speed = speed;
            m_hasRootMotion = hasRootMotion;
        }
    }
}
