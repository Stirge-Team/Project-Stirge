using UnityEngine;

namespace Stirge.AI
{
    public abstract class Condition : MonoBehaviour
    {
        public abstract bool IsTrue(Agent agent);
    }        
}