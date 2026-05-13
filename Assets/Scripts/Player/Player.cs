using UnityEngine;

namespace Stirge.Player
{
    using Combat;
    using Input;
    using Management;
    
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputProcessing))]
    public class Player : CombatEntity
    {
        [SerializeField] private PlayerMovement m_movement;
        [SerializeField] private PlayerInputProcessing m_input;
        
        protected override void AwakeThis()
        {
            base.AwakeThis();

            if(!m_movement || !m_input)
            {
                Debug.LogError("Player is missing key components. Please ensure that the movement and input scripts are attached to the player!");
            }
        }

        public void AttemptJump()
        {
            if(m_movement.OnJump())
            {
                m_health.StartInvincibility(1, EntityHealth.InvincibilityType.NoModifiations);
            }
        }

        public override void EnterStun(float stunLength)
        {
            //m_movement.Motor.HaltHorizontalVelocity(MovementMotor.SetMotorAction.NoChange);
            m_movement.Motor.SetActive(false, false, stunLength);
            m_anim.Play("hitstun");
            m_input.SetInputReading(false, stunLength);
        }
        public override void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength)
        {
            //lazy implementation - do more later
            EnterStun(stunLength);
        }
        public override void EnterKnockback(float strength, Vector3 direction, float height, float stunLength)
        {
            m_movement.Motor.ApplyForce(-transform.forward * strength + -transform.up * height, ForceMode.Impulse, true);
        }

        public override bool IsGrounded()
        {
            return m_movement.IsGrounded;
        }
        public override void ApplyRootMotion()
        {
            throw new System.NotImplementedException();
        }

        protected override Vector3 GetPosition()
        {
            return transform.position;
        }
        protected override Quaternion GetRotation()
        {
            return transform.rotation;
        }
        protected override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        protected override void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        protected override void SetRotation(Vector3 eulerRotation)
        {
            transform.rotation = Quaternion.Euler(eulerRotation);
        }

        protected override void BeginGoToPosition(Vector3 newPosition)
        {
            throw new System.NotImplementedException();
        }

        protected override void StopGoToPosition()
        {
            throw new System.NotImplementedException();
        }

        protected override float GetMovementSpeed()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetMovementSpeed(float speed)
        {
            throw new System.NotImplementedException();
        }

        protected override void ResetMovementSpeed()
        {
            throw new System.NotImplementedException();
        }
    }
}