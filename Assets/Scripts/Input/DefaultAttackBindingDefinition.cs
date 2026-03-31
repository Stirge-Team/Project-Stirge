using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Input
{
    public class DefaultAttackBindingDefinition : MonoBehaviour
    {
        private enum BindingType
        {
            Grounded,
            Air,
        }
        
        [SerializeField] private PlayerInputProcessing m_inputManager;
        [Space]

        [Header("Properties")]
        [SerializeField] private BindingType m_bindingType;
        [SerializeField] private List<AttackBinding> m_defaultBindings;

        private void Start()
        {
            switch (m_bindingType)
            {
                case BindingType.Grounded:
                    m_inputManager.SetGroundedBindings(AttackBinding.ConvertToDictionary(m_defaultBindings));
                    break;
                case BindingType.Air:
                    m_inputManager.SetAirBindings(AttackBinding.ConvertToDictionary(m_defaultBindings));
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}
