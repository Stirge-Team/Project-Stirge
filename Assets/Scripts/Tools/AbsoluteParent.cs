using UnityEngine;

namespace Stirge.Tools
{
    public static class AbsoluteParent
    {
        /// <summary>
        /// Recursivly checks the target objects' parent 
        /// </summary>
        /// <param name="targetObject">The object who's parent you're trying to get</param>
        /// <param name="nullCheckDepth">UNUSED</param>
        /// <returns></returns>
        public static Transform GetAbsoluteParent(Transform targetObject, int nullCheckDepth = 0)
        {
            Transform current = targetObject;
            while (true)
            {
                Transform parent = current.parent;
                if (parent)
                    current = current.parent;
                else return current;
            }
        }
        public static bool SharedParent(Transform[] targets, int nullCheckDepth = 0)
        {
            if(targets.Length < 2)
            {
                Debug.LogWarning("There are not enough object to compare.");
                return false;
            }
            Transform firstParent = GetAbsoluteParent(targets[0], nullCheckDepth); //get the first object's parent
            foreach(var targ in targets)
            {
                if(GetAbsoluteParent(targ,nullCheckDepth) != firstParent) //if there is a single different parent
                    return false;
            }
            return true; //if all parents are the same.
        }
    }
}
