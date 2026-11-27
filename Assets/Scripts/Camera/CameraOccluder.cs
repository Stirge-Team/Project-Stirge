using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public class CameraOccluder : CinemachineExtension
{
    [Header("Camera settings")]
    [SerializeField, Tooltip("How much space the camera needs around it")]
    private float m_cameraSize = 0.2f;
    [SerializeField, Tooltip("What collision layers to include when checking camera occlusion.")]
    private LayerMask m_occlusionMasks;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        var target = vcam.Follow;
        if(target != null)
        state.PositionCorrection += KeepLineOfSight(ref state, TargetPositionCache.GetTargetPosition(target) + TargetPositionCache.GetTargetRotation(target) * Vector3.up);

    }
    Vector3 KeepLineOfSight(ref CameraState state, Vector3 lookAtPos)
    {
        var camPos = state.GetCorrectedPosition();
        var hitInfo = new RaycastHit();
        var newPos = CheckForBlocker(camPos, lookAtPos, m_occlusionMasks, ref hitInfo);

        if(hitInfo.collider != null)
        {
            return newPos - camPos;
        }

        return Vector3.zero;
    }
    Vector3 CheckForBlocker(Vector3 cameraPos, Vector3 lookAtPos, int LayerMask, ref RaycastHit hitInfo)
    {
        var direciton = cameraPos - lookAtPos;
        var targetDistance = direciton.magnitude;

                Debug.DrawRay(lookAtPos + transform.up * m_cameraSize, direciton.normalized * targetDistance, Color.blueViolet);
                Debug.DrawRay(lookAtPos - transform.up * m_cameraSize, direciton.normalized * targetDistance, Color.blueViolet);
                Debug.DrawRay(lookAtPos + transform.right * m_cameraSize, direciton.normalized * targetDistance, Color.blueViolet);
                Debug.DrawRay(lookAtPos - transform.right * m_cameraSize, direciton.normalized * targetDistance, Color.blueViolet);
        
        if(Physics.SphereCast(lookAtPos, m_cameraSize, direciton, out hitInfo, targetDistance, LayerMask))
        //if (hits.Length > 0)
        {
            Debug.Log($"Camera is blocked by {hitInfo.collider.name}.");
            Debug.DrawRay(hitInfo.point, Vector3.up * 10, Color.green);
            //use the first object between the player and camera
            //move away from the object equal to m_cameraSize
            return hitInfo.point + hitInfo.normal * m_cameraSize;
        }
        return cameraPos;
    }
}
