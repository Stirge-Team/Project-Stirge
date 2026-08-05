using UnityEngine;
using UnityEngine.Playables;

public class AnimationStateMixerPlayable : PlayableBehaviour
{
    private AnimationStateTrack.PostPlaybackState m_postPlaybackState;

    private readonly int m_targetAnimationStateHash;
    private readonly int m_triggerParameterID;
    private readonly bool m_hasExitTrigger;

    private Animator m_boundAnimator;
    
    public AnimationStateMixerPlayable() { }
    public AnimationStateMixerPlayable(string targetAnimationStateName, string triggerParameterName)
    {
        m_targetAnimationStateHash = Animator.StringToHash(targetAnimationStateName);
        m_hasExitTrigger = triggerParameterName != string.Empty;
        if (m_hasExitTrigger)
            m_triggerParameterID = Animator.StringToHash(triggerParameterName);
    }

    public AnimationStateTrack.PostPlaybackState postPlaybackState
    {
        get => m_postPlaybackState;
        set => m_postPlaybackState = value;
    }

    public static ScriptPlayable<AnimationStateMixerPlayable> Create(PlayableGraph graph, int inputCount, string targetAnimationStateName, string triggerParameterName)
    {
        return ScriptPlayable<AnimationStateMixerPlayable>.Create(graph, new(targetAnimationStateName, triggerParameterName), inputCount);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_hasExitTrigger)
        {
            if (m_boundAnimator == null)
                return;

            switch (m_postPlaybackState)
            {
                case AnimationStateTrack.PostPlaybackState.Set:
                    m_boundAnimator.SetTrigger(m_triggerParameterID);
                    break;
                case AnimationStateTrack.PostPlaybackState.Reset:
                    m_boundAnimator.SetTrigger(m_triggerParameterID);
                    break;
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // checks if clip is playing
        bool HasInput()
        {
            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        
        if (m_boundAnimator == null)
        {
            m_boundAnimator = playerData as Animator;
        }

        if (m_boundAnimator == null)
            return;

        // if does not have exit trigger, can just worry about changing animation state
        if (!m_hasExitTrigger)
        {
            if (HasInput() && m_boundAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != m_targetAnimationStateHash)
                m_boundAnimator.Play(m_targetAnimationStateHash);
        }
        else
        {
            // if no clips are playing, set Exit trigger
            if (!HasInput())
            {
                m_boundAnimator.SetTrigger(m_triggerParameterID);
            }
            else
            {
                // if clips are playing, reset Exit trigger and enter Animation State if not already
                m_boundAnimator.ResetTrigger(m_triggerParameterID);
                if (m_boundAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != m_targetAnimationStateHash)
                    m_boundAnimator.Play(m_targetAnimationStateHash);
            }
        }
    }
}
