using UnityEngine;

namespace Stirge.AI
{
    public abstract class Behaviour : MonoBehaviour
    {
        public virtual void _Enter(Agent agent) { }
        public abstract void _Update(Agent agent);
        public virtual void _Exit(Agent agent) { }
    }
}
