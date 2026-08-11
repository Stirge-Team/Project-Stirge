using Stirge.Combat;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class MoveBehaviour : PlayableBehaviour
{
    private readonly AnimationCurve3D m_velocity;
    private readonly bool m_isLocal;

    private CombatEntity m_boundEntity;

    private MoveState m_state = MoveState.Waiting;
    private float m_duration;
    private float m_elapsedTime;

    public MoveBehaviour() { }
    public MoveBehaviour(AnimationCurve3D velocity, bool isLocal)
    {
        m_velocity = velocity;
        m_isLocal = isLocal;
    }
    
    public static ScriptPlayable<MoveBehaviour> Create(PlayableGraph graph, AnimationCurve3D velocity, bool isLocal)
    {
        return ScriptPlayable<MoveBehaviour>.Create(graph, new MoveBehaviour(velocity, isLocal));
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // This block allows us to do logic specifically when a clip stops playing.
        if (m_boundEntity != null)
        {
            float duration = (float)playable.GetDuration();
            float time = (float)playable.GetTime();
            float count = time + info.deltaTime;

            if (info.effectivePlayState == PlayState.Paused && count > duration || Mathf.Approximately(time, duration))
            {
                // Removed this to avoid Entity teleporting to end position if it is otherwise unreachable normally
                //m_boundEntity.MovePosition(m_boundEntity.GetPosition() + m_offset.Evaluate(m_duration));
                m_boundEntity.SetVelocityForAttack(Vector3.zero);
                m_state = MoveState.Waiting;
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (m_boundEntity == null)
        {
            m_boundEntity = playerData as CombatEntity;
        }

        if (m_boundEntity == null)
            return;

        // on start
        if (m_state == MoveState.Waiting)
        {
            m_state = MoveState.Moving;
            m_duration = (float)playable.GetDuration();
            m_elapsedTime = 0f;
        }
        // Update
        else
        {
            m_elapsedTime += info.deltaTime;

            // clamp to duration
            if (m_elapsedTime > m_duration)
                m_elapsedTime = m_duration;

            Vector3 targetVelocity;
            if (m_isLocal)
                targetVelocity = m_boundEntity.GetRotation() * m_velocity.Evaluate(m_elapsedTime);
            else
                targetVelocity = m_velocity.Evaluate(m_elapsedTime);

            m_boundEntity.SetVelocityForAttack(targetVelocity);
        }
    }

    private enum MoveState
    {
        Waiting,
        Moving
    }
}
