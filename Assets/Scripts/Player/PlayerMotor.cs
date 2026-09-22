using UnityEngine;

namespace Stirge.Player
{
    using Combat;

    public class PlayerMotor : CombatEntityMotor
    {
        [Header("Jump settings")]
        [SerializeField, Tooltip("How high (in units) the player should be able to jump.")]
        private float m_jumpHeight = 1;

        public Vector3 HorizontalVelocity =>
            new Vector3(
                Mathf.Floor(Rigidbody.linearVelocity.x * 100) / 100,
                0,
                Mathf.Floor(Rigidbody.linearVelocity.z * 100) / 100
            ); //{get; private set;}
        public float HorizontalSpeed => HorizontalVelocity.sqrMagnitude;
        public Vector3 HorizontalDirection => HorizontalVelocity.normalized;
        [SerializeField, Tooltip("How strong the player's input should register.")]
        private AnimationCurve m_inputStrengthScalar;
        public AnimationCurve InputStrengthScalar => m_inputStrengthScalar;
        public bool OnJump()
        {
            //If the player is considered grounded
            if (IsGrounded)
            {
                ResetVerticalVelocity();
                //Apply a force up on the player
                AddImpluse(transform.up * Mathf.Sqrt(2 * m_jumpHeight * -Physics.gravity.y));
                //Remove all coyote time
                EndCoyoteTime();
                //Grounded is not set to off here as the first check in fixed update will reset the player to being grounded in this frame
                return true;
            }
            return false;
        }

        public void AddImpluse(Vector3 force)
        {
            SetMovementState(MotorMovementState.Force);
            Rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
