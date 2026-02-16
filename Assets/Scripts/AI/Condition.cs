using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public abstract class Condition
    {        
        public abstract bool IsTrue(Agent agent);
    }
}
