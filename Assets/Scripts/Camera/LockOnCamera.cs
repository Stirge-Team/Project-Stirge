using System.Threading;
using Stirge.Combat;
using Stirge.Player;
using Unity.Cinemachine;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Stirge.Camera
{
    public class LockOnCamera : MonoBehaviour
    {
        private CinemachineTargetGroup m_groupScript;
        private LockSmith m_lockSmith;
        [Header("LockOn Settings")]
        [SerializeField, Tooltip("What transform to base the range maths on. Normally, this would be the player.")]
        private Transform m_originPoint;
        [SerializeField, Tooltip("Setting this to TRUE, will chose enemies cloest to the direction the origin point is facing as the lock on target.\nSetting this to FALSE will chose a target cloest to the center of the screen instead.")]
        private bool m_useOriginForward;
        [SerializeField]
        private float m_lockOnRange = 10f;
        [SerializeField, Min(0), Tooltip("How far the origin point and current lock on target have to get so the lock on exits.\nIf set to less then the lock on range, the range will be infinite.")]
        private float m_disengaugeRange = 20f;
        [SerializeField, Tooltip("How much the camera should face the lock on target.")]
        private float m_lockTargetCameraWeight = 1f;
        [SerializeField, Tooltip("How much space the lock on target should have around it on screen.")]
        private float m_lockTargetCameraRadius = 1f;
        [Header("Camera settings")]
        [SerializeField, Tooltip("How much space the camera needs around it")]
        private float m_cameraSize;
  
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
            if (LockSmith.Instance.IsLockedOn && m_disengaugeRange >= m_lockOnRange && Vector3.Distance(LockSmith.Instance.CurrentLockOnTarget.transform.position, m_originPoint.position) > m_lockOnRange)
            {
                LockSmith.Instance.RemoveTarget();
            }
        }
        void LateUpdate()
        {
            Debug.DrawRay(m_originPoint.position, (m_originPoint.position - transform.position).normalized * 10, Color.red);
            //Physics.SphereCastAll(m_originPoint.position, m_cameraSize, );;
        }
        public void TriggerLockOn(InputAction.CallbackContext context)
        {
            //move this to the lock smith script
            if (context.performed == true)
            {
                //leave this state if we're already in it
                if (m_groupScript.Targets.Count > 1)//CameraStateManager.Instance.State == "LockOn")
                {
                    LockSmith.Instance.RemoveTarget();
                    return;
                }

                //only perform the check if we're in the combat state
                CombatEntity closestTarget = null;
                float closestAngle = Mathf.Infinity;
                //go though all the current targets
                foreach (var ene in FindObjectsByType<CombatEntity>(FindObjectsSortMode.None))
                {
                    //find the distance between the targets and the origin point
                    float distance = Vector3.Distance(ene.transform.position, m_originPoint.position);
                    //if the target is the origin point and they aren't in range - skip 'em
                    if (distance > m_lockOnRange || ene.gameObject.transform == m_originPoint)
                        continue;

                    //find the angle between the origin and the target and compare it the last known angle
                    float newAngle = Vector3.Angle((ene.transform.position - m_originPoint.position).normalized, m_useOriginForward ? m_originPoint.forward : transform.forward);
                    //if the new angle is greater...
                    if (newAngle < closestAngle)
                    {
                        //..that is the new lock on target
                        closestTarget = ene;
                        closestAngle = newAngle;
                    }
                }
                //if there is a valid target
                if (closestTarget != null)
                {
                    Debug.Log("A lock on target has been found. Preping the group object...");
                    LockSmith.Instance.SetNew(closestTarget, LockSmith.CheckIfTargetDead);
                }

            }
        }
        public void ReciveLockOnTarget(CombatEntity target)
        {
            //reset the targets array to just the player
            m_groupScript.Targets = new() { m_groupScript.Targets[0] };
            //while I would love to add the enemy here, the CTG Target class has no constructor unfortunatly :(
            m_groupScript.AddMember(target.transform, m_lockTargetCameraWeight, m_lockTargetCameraRadius);
        }
        //For the lock on scripts
        public void ClearLockOnTarget()
        {
            ExitLockOn();
        }
        private void ExitLockOn()
        {
            Debug.Log("Leaving the lock on camera to the explore camera");
            m_groupScript.Targets = new() { m_groupScript.Targets[0] };
            //CameraStateManager.Instance.ChangeCameraState("Explore");
        }

        public void OnDrawGizmos()
        {
            if(m_originPoint)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(m_originPoint.position, m_lockOnRange);
                Gizmos.color = Color.softRed;
                Gizmos.DrawWireSphere(m_originPoint.position, m_disengaugeRange);
            }
        }
    }
}