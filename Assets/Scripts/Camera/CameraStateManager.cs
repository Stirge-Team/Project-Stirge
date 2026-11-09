using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Stirge.Camera
{
    
    //stupid cinemachine handles states weird so im doing it myself    
    public class CameraStateManager : MonoBehaviour
    {
    /*    private static bool s_debug = false;
        
        public static CameraStateManager Instance {get; private set;}
        public static CinemachineStateDrivenCamera Camera {get; private set;}
        public Animator CamAnim {get; private set;}
        public string State {get; private set;}

        public void Awake()
        {
            if(!Instance)
                Instance = this;
            else
                Destroy(transform);

            Camera = GetComponent<CinemachineStateDrivenCamera>();
            CamAnim = GetComponent<Animator>();
            ChangeCameraState("Explore");
        }

        public async void ChangeCameraState(string newState)
        {
            if (s_debug) Debug.Log($"Changing to {newState}...");
            CamAnim.SetTrigger(newState);
            State = newState;

            foreach(var otherCam in Camera.Instructions)
            {
            }
            await Awaitable.NextFrameAsync();
            while(Camera.IsBlending) await Awaitable.NextFrameAsync();
            Debug.Log($"The current active camera is {Camera.LiveChild.Name}");
        }*/
    }
}