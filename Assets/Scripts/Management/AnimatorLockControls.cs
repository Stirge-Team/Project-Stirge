using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorLockControls : MonoBehaviour
{
    public enum ListeningState
    {
        Any = 0,
        SelfOnly = 1,
        None = 2
    };
    private ListeningState m_listeningFor = ListeningState.Any;
    private Animator m_animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }
    public void ToggleListening(ListeningState state)
    {
        m_listeningFor = state;

        switch(m_listeningFor)
        {
            case ListeningState.Any:
                Debug.Log($"{name} is now listening out for any animation calls");
                break;
            case ListeningState.SelfOnly:
                Debug.Log($"{name} is listening out for its own animation calls");
                break;
            case ListeningState.None:
                Debug.Log($"{name} is not listening to any animation calls");
                break;


        }
    }

    public void PlayAnimation(string name)
    {
        if(m_listeningFor != ListeningState.None)
        {
            m_animator.Play(name);
        }
        else
        {
            Debug.Log($"Lalalala {name} isn't listening to your animation calls");
        }
    }
}
