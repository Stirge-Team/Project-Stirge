using UnityEngine;

namespace Stirge.Player
{
    public class PlayerAnimationControllerHandler : MonoBehaviour
    {
        private MovementMotor m_motor;
        private Animator m_controller;

        [SerializeField]
        private string m_movementParameter;

        [SerializeField]
        private float m_speedScale = 1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_motor = GetComponent<MovementMotor>();
            if (!m_motor)
                m_motor = GetComponentInChildren<MovementMotor>();

            m_controller = GetComponent<Animator>();
            if (!m_controller)
                m_controller = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            m_controller.SetFloat(m_movementParameter, m_motor._horizontalSpeed * m_speedScale);
        }
    }
}
