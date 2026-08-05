using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class AnimationStateBehaviour : PlayableBehaviour
{
    private readonly int m_targetAnimationStateHash;
    private readonly int m_exitParameterID;
    private readonly bool m_hasExitTrigger;

    private Animator m_boundAnimator;

    public AnimationStateBehaviour() { }
    public AnimationStateBehaviour(string targetAnimationStateName, string exitParameterName)
    {
        m_targetAnimationStateHash = Animator.StringToHash(targetAnimationStateName);
        if (exitParameterName != null && exitParameterName != string.Empty)
        {
            m_exitParameterID = Animator.StringToHash(exitParameterName);
            m_hasExitTrigger = true;
        }
        else
        {
            m_hasExitTrigger = false;
        }
    }

    public static ScriptPlayable<AnimationStateBehaviour> Create(PlayableGraph graph, string targetAnimationStateName, string exitParameterName)
    {
        return ScriptPlayable<AnimationStateBehaviour>.Create(graph, new AnimationStateBehaviour(targetAnimationStateName, exitParameterName));
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_boundAnimator == null)
            return;

        m_boundAnimator.SetTrigger(m_exitParameterID);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (m_boundAnimator == null)
        {
            m_boundAnimator = playerData as Animator;
        }

        if (m_boundAnimator == null)
            return;

        if (m_boundAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != m_targetAnimationStateHash)
        {
            // Just in case, reset trigger before entering state
            // If it is Set, then the Animation State will immediately exit
            if (m_hasExitTrigger)
                m_boundAnimator.ResetTrigger(m_exitParameterID);
            m_boundAnimator.Play(m_targetAnimationStateHash);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // This block allows us to do logic specifically when a clip stops playing.
        if (m_hasExitTrigger && Application.isPlaying)
        {
            float duration = (float)playable.GetDuration();
            float time = (float)playable.GetTime();
            float count = time + info.deltaTime;

            if (info.effectivePlayState == PlayState.Paused && count > duration || Mathf.Approximately(time, duration))
            {
                // Put end of clip logic here
                m_boundAnimator.SetTrigger(m_exitParameterID);
            }
        }

        base.OnBehaviourPause(playable, info);
    }
}
