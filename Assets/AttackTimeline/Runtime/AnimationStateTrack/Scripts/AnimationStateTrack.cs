using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Stirge.AttackTimeline
{
    [Serializable]
    [TrackClipType(typeof(AnimationStatePlayableAsset))]
    [TrackBindingType(typeof(Animator))]
    public class AnimationStateTrack : TrackAsset
    {
#if UNITY_EDITOR
        /// <summary>
        /// Gathers properties for preview
        /// </summary>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
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
        }
#endif
    }
}
