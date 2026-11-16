using UnityEngine;

namespace Stirge.Management
{
    public class GameExit : MonoBehaviour
    {
        public void Exit()
        {
            #if UNITY_EDITOR
                Debug.Log("So yeh the game would close here.");
                return;
            #endif
            Application.Quit();
        }
    }
}