using UnityEngine;

namespace Stirge.Player
{
    using Combat;
    using Input;
    using Stirge.AttackTimeline;
    using Stirge.Tools;
    using Stirge.UtilityAI;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(PlayerInputProcessing))]
    [RequireComponent(typeof(EntityHealth))]
    public class Player : CombatEntity
    {
        [Header("Player Properties")]
        [SerializeField] private PlayerInputProcessing m_input;
        [Header("Attack Snapping")]
        [SerializeField, Tooltip("The minimum angle (in degrees) to find a valid snapping target.")]
        private float m_snappingMaxAngle = 90;
        [SerializeField] private LayerMask m_attackSnappingMask;
        private Transform m_softlockTarget = null; //either move this to the camera or put the hard lock on here (along with some of the lock on code.)
        [SerializeField, Tooltip("How far until the soft lock target is cleared.")] private float m_softLockHoldRange = 2;
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

        protected override void UpdateThis()
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

        public override void UseAction(Action attackData)
        {
            if (m_softlockTarget == null || Vector3.Distance(transform.position, m_softlockTarget.position) > m_softLockHoldRange) //if the last target is out of range or dead
            {
                //clear the target as we're looking for a new one now
                m_softlockTarget = null;
                //Check for nearby enemies - range value to be pulled from the attack data later on.
                //use the attack data to determine the range to check, TODO - currently not possible
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackData.Range, transform.forward, attackData.Range, m_attackSnappingMask);

                if (hits.Length > 0)
                {
                    Debug.Log("Attempting to snap the player to a nearby target");
                    //only check against a given angle infront of the player (90 degress currently) - the player whipping around might be annoying
                    float recordAngle = m_snappingMaxAngle;
                    foreach (var hit in hits)
                    {
                        if (AbsoluteParent.GetAbsoluteParent(hit.transform).GetComponentInChildren<CombatEntity>() && hit.transform != transform) //change to hittable later
                        {
                            //check angle against player forward
                            Vector3 directionFromPlayer = (hit.transform.position - transform.position).normalized;
                            float angleFromPlayer = Vector3.Angle(directionFromPlayer, transform.forward);
                            Debug.Log($"Angle to nearby enemy, {hit.transform.name}, is: {angleFromPlayer}");
                            if (Mathf.Abs(recordAngle) > Mathf.Abs(angleFromPlayer))
                            {
                                Debug.Log($"Closer attack target found at angle {Mathf.Abs(angleFromPlayer)}");
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

            base.UseAction(attackData);
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
        protected override void OnStunned(float stunLength)
        {
            PlayerMotor.ResetHorizontalVelocity();
            m_input.SetInputReading(false, stunLength);
        }
        #endregion
    }
}