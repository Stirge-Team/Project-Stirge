using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Stirge.Camera
{
    public class EnemyGroupHandler : MonoBehaviour
    {
        [SerializeField]
        private float m_range = 10f;
        [SerializeField]
        private Transform m_rangeOrigin;
        private CinemachineTargetGroup m_groupScript;
        private bool m_isActive => CameraStateManager.Instance.State != "LockOn";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_groupScript = GetComponent<CinemachineTargetGroup>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //If in the lock on state - cancel
            if(!m_isActive)
                return;

            foreach (var ene in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
            { 
                //If this agent doesn't have a stirge enemy script - skip them
                if (ene.GetComponentInParent<Enemy.Enemy>() == null)
                    continue;

                //Get this enemy's transform
                Transform eneTrans = ene.GetComponent<Transform>();

                //Check if the distance from the origin is within range
                if (Vector3.Distance(eneTrans.position, m_rangeOrigin.position) <= m_range)
                {
                        //add the enemy to the list of targets
                        AttemptAddMember(eneTrans, 0.5f, 1f);
                }
                else
                {
                    //otherwise remove them - this works if they aren't in the list as the code will just skip over them
                    m_groupScript.RemoveMember(eneTrans);
                }
            }

            //if the current state is the combat state and the only target is the origin point
            if (m_groupScript.Targets.Count <= 1 && CameraStateManager.Instance.State == "Combat" && m_groupScript.Targets[0].Object == m_rangeOrigin)
            {
                Debug.Log("Only the origin remains. Returning to the explore camera & clearing all targets");
                //exit the combat state
                CameraStateManager.Instance.ChangeCameraState("Explore");
                //and clear the targets
                m_groupScript.Targets = new();
            }
            //otherwise, if there targets
            else if(m_groupScript.Targets.Count > 0)
            {
                //update the camera state to this one from the explore state
                if (CameraStateManager.Instance.State == "Explore")
                {
                    Debug.Log("Combat targets found, switching to combat state");
                    CameraStateManager.Instance.ChangeCameraState("Combat");
                    //and add the origin to the list
                    AttemptAddMember(m_rangeOrigin, 1f, 1);
                }
                    
            }

            //removing dead guys
            for(int i = 0; i < m_groupScript.Targets.Count; i++)
            {
                if(m_groupScript.Targets[i].Object == null)
                {
                    m_groupScript.Targets.RemoveAt(i);
                    i--;
                }
            }
        }

        private void AttemptAddMember(Transform newMember, float weight, float radius)
        {
            foreach(var target in m_groupScript.Targets)
            {
                if(newMember == target.Object)
                    return;
            }
            
            Debug.Log($"Adding {newMember.name} to the group of targets.");

            m_groupScript.AddMember(newMember, weight, radius);
        }
    }
}
