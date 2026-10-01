using UnityEngine;

namespace Stirge.Management
{
    public class AttemptToAddToCounter : MonoBehaviour
    {
        public void Trigger()
        {
            if(KillCounter.Instance)
                KillCounter.Instance.UpdateCounter();
        }
    }
}
