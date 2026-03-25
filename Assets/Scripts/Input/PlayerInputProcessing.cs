using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Stirge.Input
{
    [System.Flags]
    public enum AttackInput
    {
        A = 1,
        B = 2,
        X = 4,
        Y = 8,
        ERR = 16
    }

    public class PlayerInputProcessing : MonoBehaviour
    {
        [SerializeField] private Animator m_playerAnimator;
        
        [SerializeField] private float m_inputBufferTime = 0.2f;
        public const int MaxSequenceLength = 5;
        
        private Dictionary<AttackInput, string> m_bindings;

        // if combos are never going to have branching paths, this can just become an AttackBinding
        private AttackBinding m_comboBinding;

        private List<AttackInput> m_sequence = new();

        private float m_bufferTimer = 0;

        private void Update()
        {
            if (m_bufferTimer <= 0)
            {
                ProcessSequence();
                m_sequence.Clear();
                m_bufferTimer = m_inputBufferTime;
            }
            if (m_sequence.Count > 0)
            {
                m_bufferTimer -= Time.deltaTime;
            }
        }

        #region Bindings
        public void SetBindings(Dictionary<AttackInput, string> bindings)
        {
            m_bindings = new(bindings);
        }
        #endregion

        #region Sequence Processing
        private void ProcessSequence()
        {
            // this prioritises long inputs first by first checking input length of 'size', then 'size - 1' until it has checked each input individually (size = 1)
            // or an action has been processed, at which point it stops

            // The oldest inputs will be at the start of the sequence while the newest inputs will be at the end
            // we start by processing the oldest inputs to prioritise buttons the player pressed first
            int sequenceLength = m_sequence.Count; // = 3 { A, B, X }
            int size = sequenceLength; // = 3 // = 2 // = 1
            while (size > 0)
            {
                int index = 0;
                // this checks all possible sequences of length "size" within m_sequence
                while (size + index <= sequenceLength) // 3 + 0 = 3 // 2 + 0 = 2 // 2 + 1 = 3 // 1 + 0 = 1 // 1 + 1 = 2 // 1 + 2 = 3
                {
                    AttackInput input = 0;
                    for (int i = index; i < size + index; i++) // i = 0; i < 3; // i = 0; i < 2; // i = 1; i < 3; // i = 0; i < 1 // i = 1; i < 2 // i = 2; i < 3;
                    {
                        input |= m_sequence[i]; // A,B,X // A,B // B,X // A // B // X
                    }
                    if (ProcessInput(input))
                        return;
                    index++;
                }
                // next loop, we will check all possible sequence of length "size - 1"
                size--;
                // this continues until size is 0 or an input is processed
            }
        }

        public bool ProcessInput(AttackInput input)
        {
            if (m_comboBinding != null && m_comboBinding.attackInput == input)
            {
                m_playerAnimator.Play(m_comboBinding.attackName);
                return true;
            }
            // check if the player is in a state where they are able to attack
            else if (m_bindings.TryGetValue(input, out string attackName))
            {
                m_playerAnimator.Play(attackName);
                return true;
            }

            return false;
        }

        private void AddInputToSequence(AttackInput input)
        {
            if (m_sequence.Count == MaxSequenceLength)
                m_sequence.RemoveAt(0);
            m_sequence.Add(input);
        }
        #endregion

        #region Combos
        public void SetComboBinding(AttackBinding binding)
        {
            m_comboBinding = new(binding);
        }

        public void ClearComboBinding()
        {
            m_comboBinding = null;
        }
        #endregion

        #region Unity Input
        public void GetAInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AddInputToSequence(AttackInput.A);
            }
        }
        public void GetBInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AddInputToSequence(AttackInput.B);
            }
        }
        public void GetXInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AddInputToSequence(AttackInput.X);
            }
        }
        public void GetYInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AddInputToSequence(AttackInput.Y);
            }
        }
        #endregion
    }
}
