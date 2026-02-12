// https://www.packtpub.com/en-us/learning/how-to-tutorials/building-your-own-basic-behavior-tree-tutorial/

using UnityEngine;

namespace Stirge.AI
{
    public enum NodeStates
    {
        FAILURE,
        SUCESS,
        RUNNING
    }
    
    [System.Serializable]
    public abstract class Node
    {
        public Node() { }
        
        public delegate NodeStates NodeReturn();

        protected NodeStates m_nodeState;
        public NodeStates nodeState => m_nodeState;

        public abstract NodeStates Evaluate(Agent agent);
    }
}
