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

        [System.Serializable]
        private class BindingSet
        {   
            [Header("Properties")]
            [SerializeField] private BindingType m_bindingType;
            public BindingType Type => m_bindingType;
            [SerializeField] private List<AttackBinding> m_defaultBindings = new();
            public List<AttackBinding> Defaults => m_defaultBindings;
        }
        [SerializeField] private BindingSet[] m_bindingSets;

        private void Start()
        {
            foreach(var set in m_bindingSets)
            {
                
            switch (set.Type)
            {
                case BindingType.Grounded:
                    m_inputManager.SetGroundedBindings(AttackBinding.ConvertToDictionary(set.Defaults));
                    break;
                case BindingType.Air:
                    m_inputManager.SetAirBindings(AttackBinding.ConvertToDictionary(set.Defaults));
                    break;
            }
            }
            
            Destroy(gameObject);
        }
    }
}
