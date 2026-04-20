using UnityEngine;

namespace Stirge.Player
{
    public class PlayerAnimationControllerHandler : MonoBehaviour
    {
        [SerializeField]
        private Animator m_controller;

        [Header("Component References")]
        [SerializeField]
        private PlayerMovement m_playerMovement;
        [SerializeField]
        private MovementMotor m_motor;

        [Header("Animator Parameter Names")]
        [SerializeField]
        private string m_movementParameter;
        [SerializeField]
        private string m_isGroundedParameter;

        [Header("Properties")]
        [SerializeField]
        private float m_speedScale = 1;

        // Update is called once per frame
        void Update()
        {
            m_controller.SetFloat(m_movementParameter, m_motor._horizontalSpeed * m_speedScale);
            m_controller.SetBool(m_isGroundedParameter, m_playerMovement.IsGrounded);
        }
    }
}
