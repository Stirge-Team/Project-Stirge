using Unity.Cinemachine;
using UnityEngine;
public class CameraOccluder : CinemachineExtension
{
    [Header("Camera settings")]
    [SerializeField, Tooltip("How much space the camera needs around it")]
    private float m_cameraSize = 0.2f;
    [SerializeField, Tooltip("What collision layers to include when checking camera occlusion.")]
    private LayerMask m_occlusionMasks;
 
    void LateUpdate()
    {
        var CamFromOrigin = transform.position - ComponentOwner.Follow.position;

        Debug.DrawRay(ComponentOwner.Follow.position + transform.up * m_cameraSize, CamFromOrigin.normalized * CamFromOrigin.magnitude, Color.blueViolet);
        Debug.DrawRay(ComponentOwner.Follow.position - transform.up * m_cameraSize, CamFromOrigin.normalized * CamFromOrigin.magnitude, Color.blueViolet);
        Debug.DrawRay(ComponentOwner.Follow.position + transform.right * m_cameraSize, CamFromOrigin.normalized * CamFromOrigin.magnitude, Color.blueViolet);
        Debug.DrawRay(ComponentOwner.Follow.position - transform.right * m_cameraSize, CamFromOrigin.normalized * CamFromOrigin.magnitude, Color.blueViolet);
        var hits = Physics.SphereCastAll(ComponentOwner.Follow.position, m_cameraSize, CamFromOrigin.normalized, CamFromOrigin.magnitude, m_occlusionMasks);
        if (hits.Length > 0)
        {
            Debug.Log($"Camera is blocked by {hits[0].collider.name}.");
            Debug.DrawRay(hits[0].point, Vector3.up * 10, Color.green);
            //use the first object between the player and camera
            //move away from the object equal to m_cameraSize
            ForceCameraPosition(hits[0].point - CamFromOrigin.normalized * m_cameraSize, transform.rotation);
        }

    }
}
