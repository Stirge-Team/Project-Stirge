using System;
using Stirge.Combat;
using UnityEngine;

public class LockSmith : MonoBehaviour
{
    private CombatEntity m_currentLockOnTarget;
    public CombatEntity CurrentLockOnTarget { get { return m_currentLockOnTarget; } }
    public delegate bool RemovalCondition(CombatEntity target);
    private RemovalCondition m_currentTargetRemovalCondition;
    private bool m_awaitRemoval => m_currentTargetRemovalCondition != null;

    [SerializeField, Tooltip("A list of objects with the function \"ReciveLockOnTarget\".")]
    private Transform[] m_targetClients;

    // Update is called once per frame
    void Update()
    {
        //just checking the removal conditions for now
        //if we have a target and removal condition
        if (m_awaitRemoval && m_currentLockOnTarget != null)
        {
            //if the condition has been met
            if (m_currentTargetRemovalCondition(m_currentLockOnTarget))
            {
                RemoveTarget();
            }
        }

    }

    public bool SetNew(CombatEntity target, RemovalCondition condition = null)
    {
        var b = SetNewTarget(target);
        if (condition != null) SetNewCondition(condition);

        return b;
    }
    /// <summary>
    /// When given a new target object, will set it to the target object.
    /// </summary>
    /// <param name="newTarget">The new object to lock to</param>
    /// <returns>True if the target was set</returns>
    public bool SetNewTarget(CombatEntity newTarget)
    {
        //if the new target is different
        if (newTarget != m_currentLockOnTarget)
        {
            //set it as the new target
            Debug.Log($"Setting {newTarget.name} as the new target.", this);
            m_currentLockOnTarget = newTarget;
            UpdateClients();
            return true;
        }
        else
        {
            //else, do nothing
            Debug.Log($"{newTarget.name} is already set as the current target.", this);
            return false;
        }
    }
    /// <summary>
    /// Removes the current target
    /// </summary>
    public void RemoveTarget()
    {
        Debug.Log($"Removing target {m_currentLockOnTarget.name} as the lock on target.", this);
        m_currentLockOnTarget = null;
        RemoveCondition();
        UpdateClients();
    }
    /// <summary>
    /// Sets the current removal condition
    /// </summary>
    /// <param name="condition">The new condition to use</param>
    /// <returns>True if the condition was set.</returns>
    public bool SetNewCondition(RemovalCondition condition)
    {
        if (m_currentLockOnTarget != null)
        {
            //if the new condition is different
            if (condition != m_currentTargetRemovalCondition)
            {
                //if the new condition is already met
                if (condition(m_currentLockOnTarget))
                {
                    Debug.Log($"New removal condition is already met.");
                    RemoveTarget();
                }
                else
                {
                    //else set it as the current condition
                    Debug.Log($"Setting removal condition to {condition}.", this);
                    m_currentTargetRemovalCondition = condition;
                }
                return true;
            }
            else
            {
                //else do nothing
                Debug.Log($"Remove condition {condition} is already set as the current condition.", this);
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"There is no lock on target set! Please set one before attempting to set the removal condition", this);
            return false;
        }
    }
    /// <summary>
    /// Removes the current removal condition
    /// </summary>
    public void RemoveCondition()
    {
        Debug.Log($"Removing the {m_currentTargetRemovalCondition} condition.", this);
        m_currentTargetRemovalCondition = null;
    }

    /// <summary>
    /// Attempts to update all the listed client objects with the currently set lock on target.
    /// Yeh ik this is not great resource wise, but is only gonna be 2 objects (28/9/2026)
    /// </summary>
    private void UpdateClients()
    {
        foreach (var client in m_targetClients)
        {
            try
            {
                if(m_currentLockOnTarget != null)
                {
                    client.BroadcastMessage("ReciveLockOnTarget", m_currentLockOnTarget);
                }
                else
                {
                    client.BroadcastMessage("ClearLockOnTarget");
                }
            }
            catch (MissingMethodException e)
            {
                Debug.LogWarning($"Object {client.name} doesn't have the method \"ReciveLockOnTarget\". ({e})", this);
            }
        }
    }
    public static bool CheckIfTargetDead(CombatEntity target)
    {
        return target.IsDead();
    }
}
