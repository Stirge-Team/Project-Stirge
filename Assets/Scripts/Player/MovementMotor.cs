using UnityEngine;
using System.Collections;

namespace Stirge.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementMotor : MonoBehaviour
    {
        private Rigidbody m_rb;
        public Vector3 _horizontalVelocity =>
            new Vector3(
                Mathf.Floor(m_rb.linearVelocity.x * 100) / 100,
                0,
                Mathf.Floor(m_rb.linearVelocity.z * 100) / 100
            ); //{get; private set;}
        public float _horizontalSpeed => _horizontalVelocity.sqrMagnitude;
        public Vector3 _horizontalDirection => _horizontalVelocity.normalized;
        public float _verticalVelocity => m_rb.linearVelocity.y; // {get; private set;}
        private IEnumerator m_flipEnabled;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            m_rb = GetComponent<Rigidbody>();
        }

        ///<summary>
        ///Flips the current active state, returns the new state
        ///</summary>
        public bool Toggle(bool updateKinematic = false)
        {
            enabled = !enabled;
            if (updateKinematic)
                m_rb.isKinematic = !enabled;
            return enabled;
        }

        ///<summary>
        ///Returns true if the active state was changed to the value
        ///</summary>
        public bool SetActive(bool value, bool updateKinematic = false, float time = 0)
        {
            bool didChange = false;
            if (enabled != value)
            {
                enabled = value;
                didChange = true;
                if(time > 0)
                {
                    m_flipEnabled = FlipEnabled(time);
                    StartCoroutine(m_flipEnabled);
                }
                else if (time < 0)
                {
                    StopCoroutine(m_flipEnabled);
                }
            }

            if (updateKinematic)
                m_rb.isKinematic = !enabled;
            return didChange;
        }
        private IEnumerator FlipEnabled(float time)
        {
            yield return new WaitForSeconds(time);
            enabled = !enabled;
        } 

        public void ApplyForce(
            Vector3 force,
            ForceMode mode = ForceMode.Force,
            bool forceAction = false
        )
        {
            if (enabled || forceAction)
            {
                m_rb.AddForce(force, mode);
            }
        }

        public void ClampVerticalVelocity(float min, float max = Mathf.Infinity)
        {
            if (enabled)
            {
                if (min > m_rb.linearVelocity.y)
                {
                    m_rb.linearVelocity -= new Vector3(0, _verticalVelocity + min, 0);
                }
                if (max < m_rb.linearVelocity.y)
                {
                    m_rb.linearVelocity -= new Vector3(0, _verticalVelocity - max, 0);
                }
            }
        }

        public void ClampHorizontalVelocity(float min, float max)
        {
            if (enabled)
            {
                if (min > _horizontalSpeed)
                {
                    m_rb.linearVelocity -= new Vector3(
                        _horizontalVelocity.x + min * _horizontalDirection.x,
                        0,
                        _horizontalVelocity.z + min * _horizontalDirection.z
                    );
                }
                if (max < _horizontalSpeed)
                {
                    m_rb.linearVelocity -= new Vector3(
                        _horizontalVelocity.x - max * _horizontalDirection.x,
                        0,
                        _horizontalVelocity.z - max * _horizontalDirection.z
                    );
                }
            }
        }

        public void SetVelocity(Vector3 value)
        {
            if (enabled)
                m_rb.linearVelocity = value;
        }

        public void SetVelocity(float xVal, float yVal, float zVal)
        {
            if (enabled)
                SetVelocity(new Vector3(xVal, yVal, zVal));
        }

        public void ResetVelocity(bool doX, bool doY, bool doZ)
        {
            if (enabled)
            {
                Vector3 resetVelo = m_rb.linearVelocity;
                if (doX)
                {
                    resetVelo.x = 0;
                }
                if (doY)
                {
                    resetVelo.y = 0;
                }
                if (doZ)
                {
                    resetVelo.z = 0;
                }
                SetVelocity(resetVelo);
            }
        }

        public void HaltHorizontalVelocity(bool setMotor = false)
        {
            ResetVelocity(true, false, true);
            SetActive(setMotor);
        }

        public void RotateTo(Quaternion newRotation)
        {
            if (enabled)
            {
                transform.rotation = newRotation;
                //Stop the transform from looking at the ground
                transform.Rotate(-transform.rotation.x, 0, 0);
            }
        }
    }
}
