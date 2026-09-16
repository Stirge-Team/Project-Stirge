using UnityEngine;
using System.Collections.Generic;
using Stirge.Combat.Attacks;
using System.Linq;

namespace Stirge.Input
{
    public class PlayerInputProcessingDebug : MonoBehaviour
    {
        [SerializeField] private List<AttackInput> m_inputs;
        [SerializeField] private List<AttackData> m_datas;

        private void Update()
        {
            var dict = PlayerInputProcessing.Instance.ComboBindings.ToList();
            m_inputs = dict.Select(x => x.Key).ToList();
            m_datas = dict.Select(y => y.Value).ToList();
        }
    }
}
