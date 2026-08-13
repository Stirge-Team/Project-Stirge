using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace Stirge.AttackTimeline
{
    public class AnimationStateBehaviour : PlayableBehaviour
    {
        private readonly int m_targetAnimationStateHash;
        private readonly int m_exitParameterID;
        private readonly bool m_hasExitTrigger;
        private readonly AnimationClip m_previewClip;

        private Animator m_boundAnimator;

        public AnimationStateBehaviour() { }
        public AnimationStateBehaviour(string targetAnimationStateName, string exitParameterName, AnimationClip previewClip)
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

            m_previewClip = previewClip;
        }

        public static ScriptPlayable<AnimationStateBehaviour> Create(PlayableGraph graph, string targetAnimationStateName, string exitParameterName, AnimationClip previewClip)
        {
            return ScriptPlayable<AnimationStateBehaviour>.Create(graph, new AnimationStateBehaviour(targetAnimationStateName, exitParameterName, previewClip));
        }

#if UNITY_EDITOR
        public override void OnPlayableCreate(Playable playable)
        {
            if (!Application.isPlaying)
            {
                AnimationMode.StartAnimationMode();
            }
        }
#endif

        public override void OnPlayableDestroy(Playable playable)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                AnimationMode.StopAnimationMode();
                return;
            }
#endif
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

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TimelineEditorWindow window = TimelineEditor.GetWindow();
                // if the Timeline Editor Window is opened
                if (window != null)
                {
                    // get the current double time of the Timeline
                    double currentTime = window.playbackControls.GetCurrentTime();

                    TimelineClip activeClip = GetActiveAnimationStateClip(currentTime);

                    // If there is an active clip, preview the Animation of that clip
                    if (activeClip != null)
                    {
                        float sampleTime = (float)(currentTime - activeClip.start);
                        AnimationMode.BeginSampling();
                        AnimationMode.SampleAnimationClip(m_boundAnimator.gameObject, m_previewClip, sampleTime);
                        AnimationMode.EndSampling();
                    }
                }
                return;
            }
#endif
            if (!m_boundAnimator.IsInTransition(0) && m_boundAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != m_targetAnimationStateHash)
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
            if (m_boundAnimator != null && m_hasExitTrigger && Application.isPlaying)
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
        }

#if UNITY_EDITOR
        /// <summary>
        /// Assumes there is only one AnimationStateTrack in this TimelineAsset and that you cannot mix clips.
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns>The currently active TimelineClip of the currently active TimelineAsset's AnimationStateTrack, if such a track exists and there is a clip active at the time <paramref name="currentTime"/>.</returns>
        private TimelineClip GetActiveAnimationStateClip(double currentTime)
        {
            foreach (TrackAsset track in TimelineEditor.masterAsset.GetRootTracks())
            {
                if (track is AnimationStateTrack animationStateTrack)
                {
                    // NOTE: Potentially could use animationStateTrack to get the Preview each frame instead of saving it locally using the Constructor here
                    foreach (TimelineClip clip in animationStateTrack.GetClips())
                    {
                        if (clip.start <= currentTime && clip.end >= currentTime)
                        {
                            return clip;
                        }
                    }
                    return null;
                }
            }
            return null;
        }
#endif
    }
}
