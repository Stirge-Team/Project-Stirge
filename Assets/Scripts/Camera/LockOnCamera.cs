using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.Camera
{
public class LockOnCamera : MonoBehaviour
{
    private CinemachineTargetGroup m_groupScript;
    [SerializeField]
      private Transform m_originPoint;
      [SerializeField]
    private bool m_toggleLockOn = true;
    private bool m_lockOnActive;
    [SerializeField]
    private bool m_useOriginForward;
    [SerializeField]
    private float m_lockOnRange = 10f;
    [SerializeField]
    private bool m_lockOnDisengauge = false;
    [SerializeField]
    private bool m_lockOnTargetFailOver = false;
    [SerializeField, Range(0, 1)]
    private float m_lockOnWeight = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_groupScript = FindFirstObjectByType<CinemachineTargetGroup>();
    }

    // Update is called once per frame
    void Update()
        {
            //Check if either the target is out of range or dead
            //We can assume 0 to be the target as that is how it is added to the array in the lock on startup
            if(CameraStateManager.Instance.State == "LockOn" && (m_lockOnDisengauge || m_groupScript.Targets[0].Object == null))
            {
                if(m_groupScript.Targets[0].Object == null || Vector3.Distance(m_groupScript.Targets[0].Object.position, m_originPoint.position) > m_lockOnRange)
                {
                    CameraStateManager.Instance.ChangeCameraState("Explore");
                }
            }
        }
    public void TriggerLockOn(InputAction.CallbackContext context)
    {
        if(context.performed == true)
            //only perform the check if we're in the combat state
            if(CameraStateManager.Instance.State == "Combat")
            {
                Transform closestTarget = null;
                float closestAngle = Mathf.Infinity;
                //go though all the current targets
                foreach(var ene in m_groupScript.Targets)
                {
                    //find the distance between the targets and the origin point
                    Vector3 distance = ene.Object.position - m_originPoint.position;
                    //if the target is the origin point and they aren't in range - skip 'em
                    if(distance.sqrMagnitude > m_lockOnRange || ene.Object == m_originPoint)
                        continue;

                    //find the angle between the origin and the target and compare it the last known angle
                    float newAngle = Vector3.Angle(distance.normalized, m_useOriginForward ? m_originPoint.forward : transform.forward);
                    //if the new angle is greater...
                    if (newAngle < closestAngle)
                    {
                        //..that is the new lock on target
                        closestTarget = ene.Object;
                        closestAngle = newAngle;
                    }
                }
                //if there is a valid target
                if(closestTarget != null)
                {
                    Debug.Log("A lock on target has been found. Preping the group object...");
                    CameraStateManager.Instance.ChangeCameraState("LockOn");
                    //reset the target list and add the origin and target
                    m_groupScript.Targets = new();
                    m_groupScript.AddMember(closestTarget, m_lockOnWeight, 1f);
                    m_groupScript.AddMember(m_originPoint, 1 - m_lockOnWeight, 1f);
                }
            }
            //leave this state if we're already in it
            else if(CameraStateManager.Instance.State == "LockOn")
            {
                Debug.Log("Leaving the lock on camera to the explore camera");
                CameraStateManager.Instance.ChangeCameraState("Explore");
            }
    }
}
}