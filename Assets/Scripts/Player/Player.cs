using UnityEngine;

namespace Stirge.Player
{
    using Combat;
    using Input;
    using UnityEngine.InputSystem;

    //[RequireComponent(typeof(PlayerMovement))]
    using Management;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputProcessing))]
    [RequireComponent(typeof(EntityHealth))]
    public class Player : CombatEntity
    {
        [Header("Player Properties")]
        [SerializeField] private PlayerInputProcessing m_input;
        protected PlayerMotor Motor => (PlayerMotor)base.Motor;
        protected Vector2 m_inputDirection;
        private Transform m_camTransform;

        #region UnityEvents
        protected override void AwakeThis()
        {
            if(!Motor || !m_input || !Health)
            {
                Debug.LogError("Player is missing key components. Please ensure that the movement and input scripts are attached to the player!");
            }
            m_camTransform = UnityEngine.Camera.main.transform;
        }

        protected override void UpdateThis(float deltaTime)
        {
            if (m_isPerformingAction)
            {
                Motor.enabled = false;
            }
            else
            {
                Motor.enabled = true;
            }

            Vector3 attemptedMoveDirection = (new Vector3(m_camTransform.forward.x, 0, m_camTransform.forward.z) * m_inputDirection.y +
                                              new Vector3(m_camTransform.right.x, 0, m_camTransform.right.z) * m_inputDirection.x).normalized;
            //TODO player rotation and lock on stuff here
            if(attemptedMoveDirection.sqrMagnitude > 0)
            {
                Motor.SetRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attemptedMoveDirection), Motor.CurrentMovementProperties.AngularSpeed));
            }

            if(Motor.HorizontalSpeed < Motor.CurrentMovementProperties.HorizontalTopSpeed ||
               Vector3.Angle(Motor.HorizontalDirection, attemptedMoveDirection) > 90.0f)
            {
                Motor.AddForce(Motor.InputStrengthScalar.Evaluate(m_inputDirection.sqrMagnitude) * m_inputDirection.magnitude * transform.forward * Motor.CurrentMovementProperties.Acceleration * Time.deltaTime);
            }

            Motor.AddForce(Motor.HorizontalDirection * -Motor.CurrentMovementProperties.Friction * Mathf.Clamp01(Motor.HorizontalSpeed) * Time.deltaTime);
        }
        #endregion

        #region Inputs
        public void AttemptJump(InputAction.CallbackContext context)
        {
            if(Motor.OnJump())
            {
                Health.StartInvincibility(1, EntityHealth.InvincibilityType.NoModifiations);
            }
            if (context.performed)
                if (m_movement.OnJump())
                {
                    m_health.StartInvincibility(1, EntityHealth.InvincibilityType.NoModifiations);
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
            Motor.ResetHorizontalVelocity();
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
            Motor.AddForce(direction * strength + transform.up * height); //override
        }
        #endregion

        #region Transformation
        public override Vector3 GetPosition()
        {
            return transform.position;
        }
        public override Quaternion GetRotation()
        {
            return transform.rotation;
        }
        public Vector3 GetEulerRotation()
        {
            return transform.rotation.eulerAngles;
        }
        public override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        public override void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        protected void SetEulerRotation(Vector3 eulerRotation)
        {
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
        public override Vector3 GetForward()
        {
            return transform.forward;
        }

        public void BeginGoToPosition(Vector3 newPosition)
        {
            Vector3 direction = (newPosition - transform.position).normalized;
            Motor.AddForce(direction * Motor.CurrentMovementProperties.Acceleration);
        }
        public void StopGoToPosition()
        {
            Motor.ResetHorizontalVelocity();
        }
        #endregion
    }
}