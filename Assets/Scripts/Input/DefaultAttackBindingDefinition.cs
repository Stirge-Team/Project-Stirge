using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Input
{
    public class DefaultAttackBindingDefinition : MonoBehaviour
    {
        [SerializeField] private PlayerInputProcessing m_inputManager;
        [Space]
        [SerializeField] private List<AttackBinding> m_defaultBindings;

        private void Start()
        {
            m_inputManager.SetBindings(AttackBinding.ConvertToDictionary(m_defaultBindings));
            Destroy(gameObject);
        }
    }
}
