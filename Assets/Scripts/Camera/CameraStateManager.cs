using Unity.Cinemachine;
using UnityEngine;

namespace Stirge.Camera
{
    //stupid cinemachine handles states weird so im doing it myself    
    public class CameraStateManager : MonoBehaviour
    {
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
            State = "Explore";
        }

        public void ChangeCameraState(string newState)
        {
            Debug.Log($"Changing to {newState}...");
            CamAnim.SetTrigger(newState);
            State = newState;
        }
    }
}