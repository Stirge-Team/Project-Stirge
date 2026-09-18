using UnityEngine;

public class LockSmith : MonoBehaviour
{
    private Transform m_currentLockOnTarget;
    public Transform CurrentLockOnTarget { get { return m_currentLockOnTarget; }}
    public delegate bool RemovalCondition(Transform target);
    private RemovalCondition m_currentTargetRemovalCondition;
    private bool m_awaitRemoval => m_currentTargetRemovalCondition != null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_awaitRemoval && m_currentLockOnTarget)
        {
            if(m_currentTargetRemovalCondition(m_currentLockOnTarget))
            {
                m_currentLockOnTarget = null;
            }
        }
    }

    public bool SetNewTarget(Transform newTarget, RemovalCondition condition = null)
    {
        if(newTarget != m_currentLockOnTarget)
        {
            Debug.Log()
        }
    }
}
