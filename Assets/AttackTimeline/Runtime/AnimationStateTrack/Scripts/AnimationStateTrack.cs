using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

[Serializable]
[TrackClipType(typeof(AnimationStatePlayableAsset))]
[TrackBindingType(typeof(Animator))]
public class AnimationStateTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        /*
        if (m_targetAnimationStateName != null && m_targetAnimationStateName != string.Empty)
            clip.displayName = m_targetAnimationStateName;
        */
        clip.duration = 1d;
        base.OnCreateClip(clip);
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {

        // Create the animation mixer playable for blending clips
        AnimationMixerPlayable animMixer = AnimationMixerPlayable.Create(graph, inputCount);

        // If avatar mask is set, use a layer mixer
        Playable outputPlayable = animMixer;

        return outputPlayable;
    }

    /// <inheritdoc />
    public override IEnumerable<PlayableBinding> outputs
    {
        get { yield return AnimationPlayableBinding.Create(name, this); }
    }

    /// <summary>
    /// Gathers properties for preview
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        #if UNITY_EDITOR
        //get the bound animator
        Animator boundAnimator = null;
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track == timelineAsset.markerTrack) continue;

            boundAnimator = (Animator)director.GetGenericBinding(this);
        }
        // don't continue if no animator is attached
        if (boundAnimator == null) return;

        // Gather animation clip properties for preview
        foreach (var clip in GetClips())
        {
            AnimationStatePlayableAsset ASPAclip = clip.asset as AnimationStatePlayableAsset;
            if (ASPAclip != null && ASPAclip.TargetAnimationStateName != null)
            {
                AnimatorController controller = boundAnimator.runtimeAnimatorController as AnimatorController;

                var childStates = new List<ChildAnimatorState>();
                var animatorControllerLayers = controller.layers;

                foreach (AnimatorControllerLayer layer in animatorControllerLayers)
                {
                    childStates.AddRange(layer.stateMachine.states);
                }

                foreach (ChildAnimatorState state in childStates)
                {
                    if (state.state.name == ASPAclip.TargetAnimationStateName)
                    {
                        AnimationClip animationClip = state.state.motion as AnimationClip;

                        ASPAclip.PreviewClip = animationClip;

                        break;
                    }
                    else
                    {
                        ASPAclip.PreviewClip = null;
                    }
                }
                //idk if this meant to use "ASPAclip.PreviewClip" or the "animationClip" earlier sooooo
                driver.AddFromClip(ASPAclip.PreviewClip);
            }
        }
        #endif

        base.GatherProperties(director, driver);
    }
}
