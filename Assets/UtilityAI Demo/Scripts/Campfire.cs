using UnityEngine;

namespace Stirge.UtilityAI.Demo
{
    public class Campfire : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SphereCollider m_warmthTrigger;

        [Header("Properties")]
        [SerializeField] private float m_warmthRadius;
        [SerializeField] private float m_warmthGainRate;
        [SerializeField] private float m_logBurnRate;

        private Guy m_guy;
        private int m_currentHeldLogs;
        private float m_logBurnTimer;

        private void Start()
        {
            m_warmthTrigger.radius = m_warmthRadius;
            m_warmthTrigger.isTrigger = true;
            
            m_currentHeldLogs = 0;
            m_logBurnTimer = 0;
        }

        private void Update()
        {
            if (m_logBurnTimer > 0)
            {
                if (m_guy != null)
                {
                    m_guy.IncreaseWarmth(m_warmthGainRate * Time.deltaTime);
                }

                m_logBurnTimer -= m_logBurnRate * Time.deltaTime;

                if (m_logBurnTimer <= 0 && m_currentHeldLogs > 0)
                {
                    m_currentHeldLogs--;
                    m_logBurnTimer = 1f;
                }
            }
            else if (m_currentHeldLogs > 0)
            {
                m_currentHeldLogs--;
                m_logBurnTimer = 1f;
            }
        }

        private void OnValidate()
        {
            if (m_warmthTrigger != null)
            {
                m_warmthTrigger.radius = m_warmthRadius;
                m_warmthTrigger.isTrigger = true;
            }
        }

        public void DepositLog()
        {
            m_currentHeldLogs++;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_guy = other.GetComponentInParent<Guy>();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_guy = null;
            }
        }
    }
}