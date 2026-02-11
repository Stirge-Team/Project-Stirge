using UnityEngine;

namespace Stirge.AI
{
    public abstract class Condition : MonoBehaviour
    {        
        public abstract bool IsTrue(Agent agent);

        public NodeStates NodeIsTrue(Agent agent)
        {
            if (IsTrue(agent))
                return NodeStates.SUCESS;
            else
                return NodeStates.FAILURE;
        }
    }
}
