using UnityEngine;

namespace Stirge.Player
{
    using Combat;
    using Input;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(PlayerInputProcessing))]
    [RequireComponent(typeof(EntityHealth))]
    public class Player : CombatEntity
    {
        [Header("Player Properties")]
        [SerializeField] private PlayerInputProcessing m_input;
        protected PlayerMotor PlayerMotor => (PlayerMotor)base.Motor;
        protected Vector2 m_inputDirection;
        private Transform m_camTransform;

        #region UnityEvents
        protected override void AwakeThis()
        {
            if (!PlayerMotor || !m_input || !Health)
            {
                Debug.LogError("Player is missing key components. Please ensure that the movement and input scripts are attached to the player!");
            }
            m_camTransform = UnityEngine.Camera.main.transform;
        }

        protected override void UpdateThis(float deltaTime)
        {
            if (!IsPerformingAction)
            {
                Vector3 attemptedMoveDirection = (new Vector3(m_camTransform.forward.x, 0, m_camTransform.forward.z) * m_inputDirection.y +
                                                  new Vector3(m_camTransform.right.x, 0, m_camTransform.right.z) * m_inputDirection.x).normalized;
                //TODO player rotation and lock on stuff here
                if (attemptedMoveDirection.sqrMagnitude > 0)
                {
                    PlayerMotor.SetRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attemptedMoveDirection), PlayerMotor.CurrentMovementProperties.AngularSpeed));
                }

                if (PlayerMotor.HorizontalSpeed < PlayerMotor.CurrentMovementProperties.HorizontalTopSpeed ||
                   Vector3.Angle(PlayerMotor.HorizontalDirection, attemptedMoveDirection) > 90.0f)
                {
                    PlayerMotor.AddForce(PlayerMotor.InputStrengthScalar.Evaluate(m_inputDirection.sqrMagnitude) * m_inputDirection.magnitude * transform.forward * PlayerMotor.CurrentMovementProperties.Acceleration * Time.deltaTime);
                }
            }

            PlayerMotor.AddForce(PlayerMotor.HorizontalDirection * -PlayerMotor.CurrentMovementProperties.Friction * Mathf.Clamp01(PlayerMotor.HorizontalSpeed) * Time.deltaTime);
        }
        #endregion

        #region Inputs
        public void AttemptJump(InputAction.CallbackContext context)
        {
            if (context.performed && !IsPerformingAction)
                if (PlayerMotor.OnJump())
                {
                    Health.StartInvincibility(1, EntityHealth.InvincibilityType.NoModifiations);
                }
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            m_inputDirection = context.ReadValue<Vector2>();
        }
        #endregion

        #region DeathState
        protected override void OnDamageTaken(int damage)
        {

        }
        #endregion

        #region Status
        public override void EnterStun(float stunLength)
        {
            PlayerMotor.ResetHorizontalVelocity();
            //m_anim.Play("hitstun");
            m_input.SetInputReading(false, stunLength);
        }
        public override void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength, bool ignoreGrounded)
        {
            //lazy implementation - do more later
            EnterStun(stunLength);
        }
        public override void EnterKnockback(float strength, Vector3 direction, float height, float stunLength, bool ignoreGrounded)
        {
            PlayerMotor.AddForce(direction * strength + transform.up * height); //override
        }
        #endregion

        public void BeginGoToPosition(Vector3 newPosition)
        {
            Vector3 direction = (newPosition - transform.position).normalized;
            PlayerMotor.AddForce(direction * PlayerMotor.CurrentMovementProperties.Acceleration);
        }
        public void StopGoToPosition()
        {
            PlayerMotor.ResetHorizontalVelocity();
        }
    }
}