using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.Input
{
    using Player;
    using UtilityAI;

    [System.Flags]
    public enum AttackInput
    {
        Empty = 0,
        A = 1,
        B = 2,
        X = 4,
        Y = 8,
        L = 16,
        R = 32,
    }

    public class PlayerInputProcessing : MonoBehaviour
    {
        private const int MAX_BUFFER_LENGTH = 5;
        private const int MAX_SEQUENCE_LENGTH = 3;

        [Header("References")]
        [SerializeField] private Player m_player;

        [Header("Buffer Properties")]
        [SerializeField] private int m_bufferedInputFrameLifetime = 10;

        private AttackBindingDictionary m_groundedBindings = new();
        private AttackBindingDictionary m_airBindings = new();
        private AttackBindingDictionary m_comboBindings = new(); // if combos are never going to have branching paths, this can just become an AttackBinding

        private SerializedAction m_storedComboAction;

        private readonly List<BufferedInput> m_buffer = new() { Capacity = MAX_BUFFER_LENGTH };

        private void FixedUpdate()
        {
            // Remove old Buffered Inputs
            // Count down so that indices are preserved upon removal
            for (int i = 0, count = m_buffer.Count; i < count; i++)
            {
                // if lifetime is ended
                if (m_buffer[i].Timestamp + m_bufferedInputFrameLifetime * Time.fixedDeltaTime <= Time.time)
                {
                    m_buffer.RemoveAt(i);
                    i--;
                    count--;
                }
            }

            // process the buffer
            ProcessBuffer();
        }

        #region Bindings
        public void SetGroundedBindings(AttackBinding[] bindings)
        {
            m_groundedBindings = new(bindings.ToDictionary(binding => binding.AttackInput, binding => binding.AttackAction));
        }
        public void SetAirBindings(AttackBinding[] bindings)
        {
            m_airBindings = new(bindings.ToDictionary(binding => binding.AttackInput, binding => binding.AttackAction));
        }

        public void AddComboBinding(AttackInput comboInput, SerializedAction comboAction)
        {
            m_comboBindings.Add(comboInput, comboAction);
        }
        public void ClearComboBinding()
        {
            m_comboBindings.Clear();
        }
        #endregion

        #region Input Processing
        /// <summary>
        /// I used the top comment on this post as reference. <br/>
        /// https://www.reddit.com/r/gamedev/comments/1iw22v/comment/cb8s3vq/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
        /// </summary>
        /// <param name="newInput"></param>
        private void ProcessBuffer(AttackInput newInput = default)
        {
            // if new input is not empty, add it to the buffer
            if (newInput != 0)
            {
                // if buffer is at max size, pop oldest value (always at 0)
                if (m_buffer.Count == MAX_BUFFER_LENGTH)
                {
                    m_buffer.RemoveAt(0);
                }

                // add new value to end of list
                m_buffer.Add(new(newInput));
            }

            // if the Player has a Combo Action it needs to perform, then do not try to perform a different action
            if (m_storedComboAction != null)
                return;

            // now check the buffer for valid Action inputs
            int bufferLength = m_buffer.Count;
            int size = Mathf.Min(bufferLength, MAX_SEQUENCE_LENGTH);
            while (size > 0)
            {
                // Prioritise inputs that were performed earlier, so start at index 0, which is the earliest input
                int index = 0;

                // while the range we are checking does not extend outside the bounds of the buffer
                while (index + size <= bufferLength) // may need to be <, not <=
                {
                    // Get the input
                    AttackInput processedInput = 0;
                    m_buffer.GetRange(index, size).ForEach(buf => processedInput |= buf.Input);

                    // first check for Combo Actions
                    if (TryGetComboBinding(processedInput, out m_storedComboAction))
                    {
                        // If an action is found, it is stored in the m_storedComboAction variable
                        if (m_storedComboAction.ShouldFlushPlayerInputBuffer)
                            FlushBuffer();
                        // instead of performing the Action here, wait until the current Action's Timeline Signal emitter tells us to perform it
                        return;
                    }

                    // only check for standard Actions if the Player is not already peforming an action
                    if (!m_player.IsPerformingAction)
                    {
                        // see if the input has a valid binding
                        if (TryGetStandardBinding(processedInput, out SerializedAction foundAction))
                        {
                            Action newAction = foundAction.CreateRuntimeAction();
                            if (newAction.ShouldFlushPlayerInputBuffer)
                                FlushBuffer();
                            m_player.PerformAction(newAction);
                            return;
                        }
                    }

                    index++;
                }

                size--;
            }
        }

        private bool TryGetComboBinding(AttackInput input, out SerializedAction boundAction)
        {
            // Combo Bindings are checked first as they take priority over the standard bindings
            if (m_comboBindings.TryGetValue(input, out boundAction))
            {
                ClearComboBinding(); // clear combo binding so you can't buffer multiple of the same combo attack
                return true;
            }
            return false;
        }

        private bool TryGetStandardBinding(AttackInput input, out SerializedAction boundAction)
        {
            // grounded bindings
            if (m_player.Motor.IsGrounded)
            {
                return m_groundedBindings.TryGetValue(input, out boundAction);
            }
            // air bindings
            else
            {
                return m_airBindings.TryGetValue(input, out boundAction);
            }
        }

        private void FlushBuffer()
        {
            m_buffer.Clear();
        }

        public void TryPerformStoredComboAction()
        {
            if (m_storedComboAction != null)
            {
                m_player.PerformAction(m_storedComboAction.CreateRuntimeAction());
                m_storedComboAction = null;
            }
        }
        public void ClearStoredComboAction()
        {
            m_storedComboAction = null;
        }
        #endregion

        #region Unity Input
        public void GetAInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ProcessBuffer(AttackInput.A);
            }
        }
        public void GetBInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ProcessBuffer(AttackInput.B);
            }
        }
        public void GetXInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ProcessBuffer(AttackInput.X);
            }
        }
        public void GetYInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ProcessBuffer(AttackInput.Y);
            }
        }
        #endregion

        #region State
        public void SetInputReading(bool setTo, float time = 1)
        {
            enabled = setTo;
            if(time > 0) StartCoroutine(ReEnableInputs(time));
            else if(!enabled) Debug.LogWarning($"You have provided no time for when {name} should start reading inputs again. Please make sure that you reenable this functionality elsewhere!");
        }
        private IEnumerator ReEnableInputs(float time)
        {
            yield return new WaitForSeconds(time);
            enabled = true;
        }
        #endregion

        private class AttackBindingDictionary : Dictionary<AttackInput, SerializedAction>
        {
            public AttackBindingDictionary() : base() { }
            public AttackBindingDictionary(IEnumerable<KeyValuePair<AttackInput, SerializedAction>> collection) : base(collection) { }
            public AttackBindingDictionary(IDictionary<AttackInput, SerializedAction> dictionary) : base(dictionary) { }
        }

        [System.Serializable]
        public struct BufferedInput
        {
            [SerializeField] private AttackInput m_input;
            [SerializeField] private float m_timestamp;

            public readonly AttackInput Input => m_input;
            public readonly float Timestamp => m_timestamp;

            public BufferedInput(AttackInput input)
            {
                m_input = input;
                m_timestamp = Time.time;
            }
        }

#if UNITY_EDITOR
        public void GetDebugData(out AttackBinding[] comboBindings, out BufferedInput[] buffer)
        {
            comboBindings = m_comboBindings.ToList().ConvertAll(e => new AttackBinding(e.Key, e.Value)).ToArray();
            buffer = m_buffer.ToArray();
        }
#endif
    }
}
