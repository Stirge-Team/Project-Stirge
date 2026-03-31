using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Stirge.Camera
{
    public class EnemyGroupHandler : MonoBehaviour
    {
        private CinemachineStateDrivenCamera m_cameraRig;
        [SerializeField]
        private float m_range = 10f;
        [SerializeField]
        private Transform m_rangeOrigin;
        private CinemachineTargetGroup m_groupScript;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_cameraRig = FindFirstObjectByType<CinemachineStateDrivenCamera>();
            m_groupScript = GetComponent<CinemachineTargetGroup>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //If in the lock on state - cancel
            if(m_cameraRig.LiveChild.Name == "LockOnCam")
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
            if (m_groupScript.Targets.Count == 1 && m_cameraRig.LiveChild.Name == "CombatCam" && m_groupScript.Targets[0].Object == m_rangeOrigin)
            {
                //exit the combat state
                m_cameraRig.GetComponent<Animator>().SetTrigger("Explore");
                //and clear the targets
                m_groupScript.Targets = new();
            }
            //otherwise, if there targets
            else if(m_groupScript.Targets.Count > 0)
            {
                //update the camera state to this one from the explore state
                if (m_cameraRig.LiveChild.Name == "ExploreCam")
                {
                    m_cameraRig.GetComponent<Animator>().SetTrigger("Combat");
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

            m_groupScript.AddMember(newMember, weight, radius);
        }
    }
}
