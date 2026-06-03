using System;
using UnityEngine;
using UnityEngine.Events;

namespace Stirge.UtilityAI
{
    public class ActionBuilder : MonoBehaviour, IActionBuilder
    {
        [SerializeField] private string m_name;
        [SerializeField] private Type m_actionType;
        [SerializeField] private object[] m_parameters;
        [SerializeField] private UnityEvent m_actionEvents;

        public ActionBuilder(Type actionType, object[] parameters = null, UnityEvent actionEvents = default)
        {
            m_actionType = actionType;
            m_parameters = parameters;
            m_actionEvents = actionEvents;
        }

        public Type actionType => m_actionType;

        public Action Build()
        {
            Action action = m_parameters != null
                ? Action.Create(m_actionType, m_parameters)
                : Action.Create(m_actionType);
            action.name = m_name;
            return action;
        }
    }
}
