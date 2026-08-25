using System;
using System.Collections.Generic;
using Stirge.Combat;
using UnityEditor.Build;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Stirge.Environment
{
    public class DeathTriggerBox : SimpleTriggerBox
    {
        [SerializeField, Tooltip("Even if an object does not has an entity health component, any colliding object should still be destoryed.")]
        private bool m_destroyAnyway = false;
        protected override void EnterFunc(Collider collider)
        {
            base.EnterFunc(collider);
            var health = collider.gameObject.GetComponent<EntityHealth>(); //get collider health component

            if(health != null)
            {
                health.ModifyHealth(-9999);
            }
            else if(m_destroyAnyway)
            {
                Destroy(collider.gameObject);
            }
        }
    }
}
