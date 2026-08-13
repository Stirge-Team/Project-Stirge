using Stirge.Enemy;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace Stirge.AttackTimeline
{
    using Tools;

    public class MoveBehaviour : PlayableBehaviour
    {
        private readonly AnimationCurve3D m_translation;
        private readonly bool m_isLocal;

        private EnemyMotor m_boundMotor;

        private MoveState m_state = MoveState.Waiting;
        private float m_duration;
        private float m_elapsedTime;
        private Vector3 m_lastTargetTranslation;

#if UNITY_EDITOR
        private Vector3 m_previewInitialPosition;
        private Vector3 m_previewLastTranslation;
        private double m_previewLastSampleTime;
#endif

        public MoveBehaviour() { }
        public MoveBehaviour(AnimationCurve3D translation, bool isLocal)
        {
            m_translation = translation;
            m_isLocal = isLocal;
        }

        public static ScriptPlayable<MoveBehaviour> Create(PlayableGraph graph, AnimationCurve3D translation, bool isLocal)
        {
            return ScriptPlayable<MoveBehaviour>.Create(graph, new MoveBehaviour(translation, isLocal));
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_boundMotor != null)
            {
                if (Application.isPlaying)
                {
                    if (m_state == MoveState.Moving)
                        m_boundMotor.OnAttackEnd();
                }
                else
                {
                    m_boundMotor.transform.parent.position = m_previewInitialPosition;
                }
            }
        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // This block allows us to do logic specifically when a clip stops playing.
            if (m_boundMotor != null && Application.isPlaying)
            {
                float duration = (float)playable.GetDuration();
                float time = (float)playable.GetTime();
                float count = time + info.deltaTime;

                if (info.effectivePlayState == PlayState.Paused && count > duration || Mathf.Approximately(time, duration))
                {
                    // Removed this to avoid Entity teleporting to end position if it is otherwise unreachable normally
                    //m_boundEntity.SetPosition(m_translation.Evaluate(m_duration));
                    m_boundMotor.OnAttackEnd();
                    m_state = MoveState.Waiting;
                }
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_boundMotor == null)
            {
                m_boundMotor = playerData as EnemyMotor;
            }

            if (m_boundMotor == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (m_state == MoveState.Waiting)
                {
                    m_state = MoveState.Moving;
                    m_previewInitialPosition = m_boundMotor.transform.parent.position;
                    m_previewLastTranslation = Vector3.zero;
                    m_previewLastSampleTime = 0d;
                }

                TimelineEditorWindow window = TimelineEditor.GetWindow();
                // if the Timeline Editor Window is opened
                if (window != null)
                {
                    // get the current double time of the Timeline
                    double currentTime = window.playbackControls.GetCurrentTime();

                    TimelineClip activeClip = GetActiveMoveClip(currentTime);

                    // If there is an active clip, preview the Animation of that clip
                    if (activeClip != null)
                    {
                        double sampleTime = currentTime - activeClip.start;
                        Vector3 targetTranslation;
                        if (m_isLocal)
                            targetTranslation = m_boundMotor.transform.rotation * m_translation.Evaluate((float)sampleTime);
                        else
                            targetTranslation = m_translation.Evaluate((float)sampleTime);

                        /*
                        if (sampleTime > m_previewLastSampleTime)
                            m_previewAccumulatedTranslation += targetTranslation - m_previewLastTranslation;
                        else if (sampleTime < m_previewLastSampleTime)
                            m_previewAccumulatedTranslation -= m_previewLastTranslation - targetTranslation;
                        */

                        if (sampleTime > m_previewLastSampleTime)
                            m_boundMotor.transform.parent.position = m_boundMotor.transform.parent.position + targetTranslation - m_previewLastTranslation;
                        else if (sampleTime < m_previewLastSampleTime)
                            m_boundMotor.transform.parent.position = m_boundMotor.transform.parent.position - m_previewLastTranslation + targetTranslation;

                        m_previewLastTranslation = targetTranslation;
                        m_previewLastSampleTime = sampleTime;

                    }
                }
                return;
            }
#endif

            // on start
            if (m_state == MoveState.Waiting)
            {
                m_state = MoveState.Moving;
                m_duration = (float)playable.GetDuration();
                m_elapsedTime = 0f;
                m_lastTargetTranslation = Vector3.zero;
                m_boundMotor.OnAttackStart();
            }
            // Update
            else
            {
                m_elapsedTime += info.deltaTime;

                // clamp to duration
                if (m_elapsedTime > m_duration)
                    m_elapsedTime = m_duration;

                Vector3 targetTranslation;
                if (m_isLocal)
                    targetTranslation = m_boundMotor.transform.rotation * m_translation.Evaluate(m_elapsedTime);
                else
                    targetTranslation = m_translation.Evaluate(m_elapsedTime);

                // targetTranslation - m_lastTargetTranslation = motion for this frame
                m_boundMotor.SetPosition(m_boundMotor.transform.position + targetTranslation - m_lastTargetTranslation);

                m_lastTargetTranslation = targetTranslation;
            }
        }

        private enum MoveState
        {
            Waiting,
            Moving
        }

#if UNITY_EDITOR
        /// <summary>
        /// Assumes there is only one MoveTrack in this TimelineAsset and that you cannot mix clips.
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns>The currently active TimelineClip of the currently active TimelineAsset's MoveTrack, if such a track exists and there is a clip active at the time <paramref name="currentTime"/>.</returns>
        private TimelineClip GetActiveMoveClip(double currentTime)
        {
            foreach (TrackAsset track in TimelineEditor.masterAsset.GetRootTracks())
            {
                if (track is MoveTrack moveTrack)
                {
                    foreach (TimelineClip clip in moveTrack.GetClips())
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
