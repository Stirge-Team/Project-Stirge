using UnityEngine;
using UnityEngine.AI;

namespace Stirge.Environment
{
    public class LockdownDoor : MonoBehaviour
    {
        private Animator m_mdlAnim;
        [SerializeField]
        private bool m_locked;
        private Collider m_col;
        private NavMeshObstacle m_navCutout;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_col = GetComponent<Collider>();
            m_mdlAnim = GetComponentInChildren<Animator>();
            m_navCutout = GetComponent<NavMeshObstacle>();

            ToggleLockedDoor(m_locked ? 1 : 0);
        }

        /// <summary>
        /// Changes the lock state of this door.
        /// </summary>
        /// <param name="forceState">Sets the lock state rather than fliping it. 0 == Unlock, 1 == Lock</param>
        /// <returns></returns>
        public bool ToggleLockedDoor(int forceState = -1)
        {
            
            m_locked = forceState switch
            {
                0 => false,
                1 => true,
                _ => !m_locked,
            };

            m_col.enabled = m_locked ? true : false;
            m_navCutout.enabled = m_col.enabled;
            m_mdlAnim.SetTrigger(m_locked ? "Lock" : "Unlock");
            return m_locked;
        }
        public void UnlockDoor()
        {
            ToggleLockedDoor(0);
        }
        public void LockDoor()
        {
            ToggleLockedDoor(1);
        }
    }
}