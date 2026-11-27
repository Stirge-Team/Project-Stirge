using UnityEngine;

namespace Stirge.Player
{
    using Combat;

    public class PlayerMotor : CombatEntityMotor
    {
        [Header("Player References")]
        [SerializeField] private Player m_player;

        [Header("Player Movement Properties")]
        [SerializeField, Tooltip("How high (in units) the player should be able to jump.")]
        private float m_jumpHeight = 1;
        [SerializeField, Min(0f)] private float m_coyoteTime;
        private float m_coyoteCountdown;
        [SerializeField, Tooltip("How strong the player's input should register.")]
        private AnimationCurve m_inputStrengthScalar;

        [Header("Player Animation Properties")]
        [SerializeField] private string m_speedPropertyName;
        [SerializeField] private string m_isGroundedPropertyName;
        [SerializeField] private string m_hitstunAnimName;
        [SerializeField] private string m_deathAnimName;

        private int m_speedPropertyHash;
        private int m_isGroundedPropertyHash;
        private int m_hitstunAnimHash;
        private int m_deathAnimHash;

        private float m_speedSmoothVelocity;

        // public properties
        public Vector3 HorizontalVelocity =>
            new Vector3(
                Mathf.Floor(Rigidbody.linearVelocity.x * 100) / 100,
                0,
                Mathf.Floor(Rigidbody.linearVelocity.z * 100) / 100
            ); //{get; private set;}
        public float HorizontalSpeed => HorizontalVelocity.sqrMagnitude;
        public Vector3 HorizontalDirection => HorizontalVelocity.normalized;
        public AnimationCurve InputStrengthScalar => m_inputStrengthScalar;

        #region Unity Events
        protected override void OnAwake()
        {
            m_speedPropertyHash = Animator.StringToHash(m_speedPropertyName);
            m_isGroundedPropertyHash = Animator.StringToHash(m_isGroundedPropertyName);
            m_hitstunAnimHash = Animator.StringToHash(m_hitstunAnimName);
            m_deathAnimHash = Animator.StringToHash(m_deathAnimName);
        }
        protected override void OnUpdate()
        {
            animator.SetBool(m_isGroundedPropertyHash, IsGrounded);
            
            if (m_coyoteCountdown > 0)
            {
                m_coyoteCountdown -= Time.deltaTime;
            }

            if (!IsGrounded || m_player.IsPerformingAction)
            {
                animator.SetFloat(m_speedPropertyHash, 0f);
            }
            else
            {
                float newValue = Mathf.SmoothDamp(animator.GetFloat(m_speedPropertyHash), HorizontalSpeed / groundedMovementProperties.HorizontalTopSpeed, ref m_speedSmoothVelocity, 0.1f); 
                animator.SetFloat(m_speedPropertyHash, newValue);
            }
        }

        protected override void OnEnableInternal()
        {
            m_player.OnHit += OnHit;
        }
        protected override void OnDisableInternal()
        {
            m_player.OnHit -= OnHit;
        }
        #endregion

        #region Physics
        protected override void OnBecomeNotGrounded()
        {
            // if leaving the ground, start coyote countdown
            m_coyoteCountdown = m_coyoteTime;
        }
        protected override void OnBecomeGrounded()
        {
            SetMovementState(MotorMovementState.Kinematic);
        }

        public bool AddForceClamped(Vector3 force, float minimumStrength = 0)
        {
            if (force.magnitude > minimumStrength) { AddForce(force); return true; }
            else return false;
        }
        public void AddImpluse(Vector3 force)
        {
            SetMovementState(MotorMovementState.Force);
            Rigidbody.AddForce(force, ForceMode.Impulse);
        }
        #endregion

        #region Combat
        private void OnHit(CombatEntity attacker, Vector3 hitPosition, Vector3 hitVelocity)
        {
            //m_targetHeading = -hitVelocity.normalized;

            if (m_player.Health._isDead)
                animator.Play(m_deathAnimHash);
            else
                animator.Play(m_hitstunAnimHash, 0, 0f);
        }

        protected override void OnMovementStateChangedToAction()
        {
            base.OnMovementStateChangedToAction();
            animator.SetFloat(m_speedPropertyHash, 0f);
        }
        #endregion

        #region Inputs
        public bool OnJump()
        {
            //If the player is considered grounded
            if (IsGrounded)
            {
                ResetVerticalVelocity();
                //Apply a force up on the player
                AddImpluse(transform.up * Mathf.Sqrt(2 * m_jumpHeight * -Physics.gravity.y));
                //Remove all coyote time
                m_coyoteTime = 0f;
                //Grounded is not set to off here as the first check in fixed update will reset the player to being grounded in this frame
                return true;
            }
            return false;
        }
        #endregion
    }
}
