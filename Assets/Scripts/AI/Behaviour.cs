using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public abstract class Behaviour : MonoBehaviour
    {
        public virtual void _Enter(Agent agent) { Debug.Log($"Entered {name} behaviour."); }
        public virtual void _Update(Agent agent) { }
        public virtual void _Exit(Agent agent) { Debug.Log($"Exited {name} behaviour."); }
    }
}
