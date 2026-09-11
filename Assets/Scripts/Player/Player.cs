using UnityEngine;

namespace Stirge.Player
{
    using Combat;
    using Input;
    using Stirge.Combat.Attacks;
    using Stirge.Tools;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputProcessing))]
    public class Player : CombatEntity
    {
        [Header("Player Properties")]
        [SerializeField] private PlayerMovement m_movement;
        [SerializeField] private PlayerInputProcessing m_input;
        [Header("Attack Snapping")]
        [SerializeField, Tooltip("The minimum angle (in degrees) to find a valid snapping target.")]
        private float m_snappingMaxAngle = 90;
        [SerializeField] private LayerMask m_attackSnappingMask;
        private Transform m_softlockTarget = null; //either move this to the camera or put the hard lock on here (along with some of the lock on code.)
        [SerializeField, Tooltip("How far until the soft lock target is cleared.")] private float m_softLockHoldRange = 2;

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

        public override void UseAttack(AttackData attackData)
        {
            if (m_softlockTarget == null || Vector3.Distance(transform.position, m_softlockTarget.position) > m_softLockHoldRange) //if the last target is out of range or dead
            {
                //clear the target as we're looking for a new one now
                m_softlockTarget = null;
                //Check for nearby enemies - range value to be pulled from the attack data later on.
                //use the attack data to determine the range to check, TODO - currently not possible
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 2f, transform.forward, 2f, m_attackSnappingMask);

                if (hits.Length > 0)
                {
                    //only check against a given angle infront of the player (90 degress currently) - the player whipping around might be annoying
                    float recordAngle = m_snappingMaxAngle;
                    foreach (var hit in hits)
                    {
                        if (AbsoluteParent.GetAbsoluteParent(hit.transform).GetComponent<CombatEntity>() && hit.transform != transform) //change to hittable later
                        {
                            //check angle against player forward
                            Vector3 directionFromPlayer = (hit.transform.position - transform.position).normalized;
                            float angleFromPlayer = Vector3.Angle(directionFromPlayer, transform.forward);
                            Debug.Log($"Angle to nearby enemy, {hit.transform.name}, is: {angleFromPlayer}");
                            if (Mathf.Abs(recordAngle) > Mathf.Abs(angleFromPlayer))
                            {
                                m_softlockTarget = hit.transform;
                                recordAngle = angleFromPlayer;
                            }
                        }
                    }
                }
            }
            //hard rotate the player towards valid target if there is one
            if (m_softlockTarget != null)
            {
                transform.LookAt(new Vector3(m_softlockTarget.position.x, transform.position.y, m_softlockTarget.position.z)); //stupid ngl
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
        #endregion
    }
}