using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Timeline;
using UnityEngine;

namespace Stirge.AttackTimeline
{
    [CustomEditor(typeof(AnimationStatePlayableAsset))]
    public class AnimationStatePlayableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //i have no idea how this works with multiselect so i'm not even going to bother trying to make it work
            try
            {
                //get animator binding 
                //get names of all animations attached to animator
                //put them in a dropdown
                //selecting dropdown simply edits string

                AnimationStatePlayableAsset obj = target as AnimationStatePlayableAsset;

                //get currently selected track
                AnimationStateTrack track = TimelineEditor.selectedClip.GetParentTrack() as AnimationStateTrack;

                //get currently selected bound animator
                Animator boundAnimator = TimelineEditor.inspectedDirector.GetGenericBinding(track) as Animator;

                //get controller from animator
                AnimatorController controller = boundAnimator.runtimeAnimatorController as AnimatorController;

                var childStates = new List<ChildAnimatorState>();
                var animatorControllerLayers = controller.layers;

                foreach (AnimatorControllerLayer layer in animatorControllerLayers)
                {
                    childStates.AddRange(layer.stateMachine.states);
                }

                List<string> names = new()
                {
                    "Not Found" // Null/nothing state 
                };

                foreach (ChildAnimatorState state in childStates)
                {
                    names.Add(state.state.name);
                }

                int currentlySelected = 0;
                int count = names.Count;
                for (int i = 0; i < count; i++)
                {
                    if (names[i] == obj.TargetAnimationStateName)
                    {
                        currentlySelected = i;
                        break;
                    }
                }

                Undo.RecordObject(obj, "Changed Animation State Playable");

                EditorGUI.BeginChangeCheck();

                currentlySelected = EditorGUILayout.Popup("Target Animation", currentlySelected, names.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    if (currentlySelected == 0) { return; }

                    obj.TargetAnimationStateName = names[currentlySelected];

                    EditorUtility.SetDirty(obj);
                }

                EditorGUI.BeginChangeCheck();

                string exitParameter = EditorGUILayout.DelayedTextField("Exit Parameter Name", obj.ExitParameterName);

                if (EditorGUI.EndChangeCheck())
                {
                    obj.ExitParameterName = exitParameter;

                    EditorUtility.SetDirty(obj);
                }
            }
            catch
            {
                //fallback if anything errors out
                base.OnInspectorGUI();
            }

            
        }
    }

}


