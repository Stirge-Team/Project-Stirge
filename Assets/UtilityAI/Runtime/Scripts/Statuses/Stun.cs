using Stirge.Combat;
using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    public abstract class Stun : Status, ISetupable<string>
    {
        // the name of the animation to play on the 
        private int m_animationStateHash;

        // public properties
        public sealed override StatusDurationType DurationType => StatusDurationType.Timed;
        public sealed override StatusStackType StackType => StatusStackType.Single;

        public void Setup(string animationStateName)
        {
            m_animationStateHash = Animator.StringToHash(animationStateName);
        }

        protected override void OnApplyInternal(CombatEntity inflicter, CombatEntity target)
        {
            PlayStunAnimation(target);
            target.StartStun(m_duration);
        }

        protected override void OnClearInternal(CombatEntity target)
        {
            target.StopStun();
        }

        public override bool ShouldThisClear(CombatEntity target)
        {
            return m_countdown <= 0f;
        }

        protected void PlayStunAnimation(CombatEntity target)
        {
            target.Motor.Animator.Play(m_animationStateHash);
        }
    }
}
