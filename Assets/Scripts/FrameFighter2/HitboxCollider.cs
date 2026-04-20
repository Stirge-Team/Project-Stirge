using UnityEngine;
using UnityEngine.Events;

//IMPORTANT NOTE:
//THIS SCRIPT IS VERY TEMPORARY AND WILL CHANGE WHEN I IMPLEMENT CUSTOM DATA
namespace FrameFighter2.Hitbox
{
    [RequireComponent(typeof(Collider))]
    public class HitboxCollider : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_onCollision; //temp

        public void Invoke()
        {
            m_onCollision.Invoke();
        }
    }

}

