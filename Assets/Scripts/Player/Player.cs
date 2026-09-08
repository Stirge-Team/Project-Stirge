using UnityEngine;

namespace Stirge.Player
{
    using Combat;
    using Input;
    using Stirge.Combat.Attacks;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputProcessing))]
    public class Player : CombatEntity
    {
        [Header("Player Properties")]
        [SerializeField] private PlayerMovement m_movement;
        [SerializeField] private PlayerInputProcessing m_input;

        #region UnityEvents
        protected override void AwakeThis()
        {
            if (!m_movement || !m_input)
            {
                Debug.LogError("Player is missing key components. Please ensure that the movement and input scripts are attached to the player!");
            }
        }

        protected override void UpdateThis(float deltaTime)
        {

        }
        #endregion

        #region Inputs
        public void AttemptJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                if (m_movement.OnJump())
                {
                    m_health.StartInvincibility(1, EntityHealth.InvincibilityType.NoModifiations);
                }
        }
        struct targetAngleData
        {
            public Transform transform;
            public float angle;
            public targetAngleData(Transform obj, float a)
            {
                transform = obj;
                angle = a;
            }
        }

        public override void UseAttack(AttackData attackData)
        {

            //Check for nearby enemies - range value to be pulled from the attack data later on.
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 5f, Vector3.zero);

            if (hits.Length > 0)
            {
                //only check against a given angle infront of the player (90 degress currently) - the player whipping around might be annoying
                targetAngleData currentSelection = new(null, Mathf.PI / 2);
                foreach (var hit in hits)
                {
                    //use the attack data to determine the range to check, maybe
                    //check angle against player forward
                    Vector3 directionFromPlayer = (hit.transform.position - transform.position).normalized;

                    float angleFromPlayer = Vector3.Angle(directionFromPlayer, transform.forward);
                    Debug.Log($"Angle to nearby enemy, {hit.transform.name}, is: {angleFromPlayer}");
                    if (Mathf.Abs(currentSelection.angle) > Mathf.Abs(angleFromPlayer)) currentSelection = new(hit.transform, angleFromPlayer);
                }
                //hard rotate the player towards valid target if found
                transform.LookAt(currentSelection.transform); //stupid ngl
            }
            base.UseAttack(attackData);
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
            m_movement.Motor.HaltHorizontalVelocity(MovementMotor.SetMotorAction.Off, stunLength);
            m_anim.Play("hitstun");
            m_input.SetInputReading(false, stunLength);
        }
        public override void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength, bool ignoreGrounded)
        {
            //lazy implementation - do more later
            EnterStun(stunLength);
        }
        public override void EnterKnockback(float strength, Vector3 direction, float height, float stunLength, bool ignoreGrounded)
        {
            m_movement.Motor.ApplyForce(direction * strength + transform.up * height, ForceMode.Impulse, true);
        }

        public override bool IsGrounded()
        {
            return m_movement.IsGrounded;
        }
        #endregion

        #region Transformation
        protected override Vector3 GetPosition()
        {
            return transform.position;
        }
        protected override Quaternion GetRotation()
        {
            return transform.rotation;
        }
        protected Vector3 GetEulerRotation()
        {
            return transform.rotation.eulerAngles;
        }
        protected override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        protected override void SetRotation(Quaternion rotation)
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

        protected override void BeginGoToPosition(Vector3 newPosition)
        {
            Vector3 direction = (newPosition - transform.position).normalized;
            m_movement.Motor.ApplyForce(direction * m_movement._currentStateSettings._horizontalAcceleration);
        }
        protected override void StopGoToPosition()
        {
            m_movement.Motor.HaltHorizontalVelocity(MovementMotor.SetMotorAction.NoChange);
        }

        protected override float GetMovementSpeed()
        {
            return m_movement.Motor._horizontalSpeed;
        }

        public override void ApplyPhysicsToTransform()
        {
            //nothing has to be done here - function name unclear?
        }

        public override void ApplyRootMotion()
        {
            //nothing needs to be done here also?
        }
        #endregion
    }
}