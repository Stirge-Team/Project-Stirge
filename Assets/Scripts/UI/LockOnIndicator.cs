using UnityEngine;

public class LockOnIndicator : MonoBehaviour
{
    private Transform m_target;
    public Transform uiElement;
    void Update()
    {
        if(m_target)
        {
            uiElement.transform.position = Camera.main.WorldToScreenPoint(m_target.position);
        }
    }
    public bool ChangeTarget(Transform newTarget)
    {
        if(newTarget == m_target)
        {
            Debug.LogWarning("New lock on target same as old target.");
            return false;
        }
        
        m_target = newTarget;
        return true;
    }
}
