using UnityEngine;
using System;
using System.Collections.Generic;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    using Callbacks;

    public class EnemyBlackboardUpdater : MonoBehaviour
    {
        [SerializeField] private UtilityEnemy m_enemy;
        [SerializeField] private List<CallbackBinding_Base> m_callbacks = new();

        private void Start()
        {
            if (m_enemy == null)
                m_enemy = gameObject.GetComponent<UtilityEnemy>();
        }

        private void Update()
        {
            foreach (var callback in m_callbacks)
            {
                Type valueType = callback.valueType;
                BlackboardPropertyName name = callback.propertyName;
                var objectValue = callback.GetObjectValue();

                m_enemy.Blackboard.SetObjectValue(valueType, name, objectValue);
            }
        }

        [ContextMenu("Print Blackboard")]
        public void PrintBlackboard()
        {
            Blackboard blackboard = m_enemy.Blackboard;

            if (true) // print ALL Blackboard properties
            {
                List<Type> types = new();
                blackboard.GetValueTypes(types);

                for (int i = 0, count = types.Count; i < count; i++)
                {
                    Type type = types[i];
                    List<KeyValuePair<BlackboardPropertyName, object>> properties = new();
                    blackboard.GetObjectProperties(properties);

                    string msg = type.ToString() + ": \n";
                    foreach (var property in properties)
                    {
#if SIMPLE_BLACKBOARD_SAVE_NAMES
                    msg += $"           {property.Key.name}: {property.Value}\n";
#else
                        msg += $"           {type.ToString()}: {property.Value.ToString()}\n";
#endif
                    }
                    Debug.Log(msg);
                }
            }
        }
    }
}
