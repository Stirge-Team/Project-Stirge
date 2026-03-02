using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Strige.Camera {
public class TrackingCamera : MonoBehaviour
{
    //camera needs to track between 2 objects
    //one object will be the primary target (the player 99%) - used for the position of the camera
    //the other will be the secondary target - used for the math on where the camera points
    //these objects should be able to be set by other scripts (maybe in editor but def in scripts)
    //speed, relative position are variables (idk ill figure it out as i go trust me bro)

    //Stuff from the doc Henry wrote up
    //CORE
    // - Player always visable
    // - Player avoid edges of screen
    // - Wide viewing angle
    // - Lerp the camera Movement
    // - Ease in, out or both?
    // DEFAULT
    // - Sit behind player
    // - Adjustable angle and position
    // - Player should sit low on the screen
    // COMBAT
    // - Keep surroundings, player & enemies on screen
    // - This and LO are gonna be similar
    // - Camera should adjust the number of enemies smoothly
    // - Account for multiple targets
    // - Camera should zoom to account for many foes
    // - Max and Min ranges for the zooming
    // Lock-ON (LAST)
    // - Frame the target and the player
    // - I'm picturing the pokemon combat screen positions for the player and the target
    // - Don't put the camera too far behind the player
    // - Keep the smooth movement

    [Header("Position and Movement")]
    [SerializeField, Tooltip("The speed at which the camera will move to it's desiered position")]
    private float m_cameraMovementSpeed = 1;
    [SerializeField, Tooltip("The speed at which the camera will rotate to face it's target")]
    private float m_cameraRotationSpeed = 60;
    [SerializeField, Tooltip("The position relative to the look position that the camera should be")]
    private Vector2 m_relativePosition;
    [SerializeField, Tooltip("The speed at which the camera pivots around it's origin")]
    private float m_cameraPivotSpeed = 1;
    [SerializeField, Tooltip("How much the distance between the desiered and target camera angle should affect the pivot speed.")]
    private float m_pivotGapStrength = 1;
    private float m_viewAngle;

    [Header("Auto-Movement Settings")]
    [SerializeField, Tooltip("How long the game should respect the player's change to the camera position before attempting to move itself")]
    private float m_cameraMoveWait = 3;
    [SerializeField, Tooltip("The speed at which the camera AUTO-pivots around it's origin")]
    private float m_cameraAutoPivotSpeed = 0.3f;
    private float m_cameraMoveCountdown;

    [Header("Group Target Settings")]
    [SerializeField, Tooltip("How strong the distance scaling should be during group situations (multiplicative)")]
    private float m_groupDistanceScaling = 1;
    [SerializeField, Tooltip("How far until a target is out of range of the player in combat")]
    private float m_combatRangeCutoff;
    [SerializeField, Tooltip("How biased the camera should be to enemies in range while in the combat camera mode (should it look closer to the enemies or the player?)"), Range(0, 1)]
    private float m_clumpBias = 0.5f;

    [Header("Targets")]
    [SerializeField, Tooltip("The primary target for the camera - used for position and as the default object to look at")]
    private Transform m_primaryTarget;
    [SerializeField, Tooltip("The secondary targets for the camera - used for the math on where the camera points. The 0th index of the array is for the lockon target")]
    private Transform[] m_secondaryTargets;

    [Header("Inputs")]
    [SerializeField, Tooltip("How sensitive the camera movement is")]
    private float m_inputSensitivity;
    private float m_playerCameraRotationInput;

    private Vector3 m_cameraDesiredPosition;
    private Vector3 m_cameraDesiredLookPoint;
    private List<Transform> m_cleanTargetList = new();
    private float m_desiredAngle;

    public enum CameraStates
    {
        Explore,
        Combat,
        LockOn
    };
    private CameraStates m_camState;

    private void Awake()
    {
      m_camState = CameraStates.Explore;
    }

    void Update()
    {
      switch (m_camState)
      {
        case CameraStates.Explore:

          //Find the distance between the camera and the primary target
          float primaryDistanceFromCamera = (new Vector3(m_primaryTarget.position.x - transform.position.x, 0, m_primaryTarget.position.z - transform.position.z)).magnitude;

          //Take the primary target position and add the relative position values to get the camera position
          m_cameraDesiredPosition = m_primaryTarget.position + new Vector3(
            Mathf.Cos(m_viewAngle) * m_relativePosition.x, //Move back from the centre point given the current angle
            m_relativePosition.y, //just moves it up
            Mathf.Sin(m_viewAngle) * m_relativePosition.x); //Same as the other

          //Get the distance from the desired position and primary target (ignoring the Y), and look in that direction, so its looking a bit past the target
          m_cameraDesiredLookPoint = m_primaryTarget.position + (m_primaryTarget.position - new Vector3(m_cameraDesiredPosition.x, m_primaryTarget.position.y, m_cameraDesiredPosition.z)).normalized * primaryDistanceFromCamera;
          break;

        case CameraStates.Combat:
          //Target check - Exits if there are no targets
          if(m_secondaryTargets.Length == 0)
          {
            m_camState = CameraStates.Explore;
            Debug.LogWarning("There are no combat targets for the camera to track... take your schizo pills (or check your code)");
            break;
          }

          m_cleanTargetList = new();
          //Check for any targets in range
          foreach(var targ in m_secondaryTargets)
          {
            if((m_primaryTarget.position - targ.position).magnitude < m_combatRangeCutoff)
            {
              m_cleanTargetList.Add(targ);
            }
          }
          //If all targets are out of range, stop the combat camera
          if(m_cleanTargetList.Count == 0)
          {
            m_camState = CameraStates.Explore;
            Debug.LogWarning("There are no combat targets in range... player ran away!");
            break;
          }

          //Get the average position of all valid targets - called the clump
          Vector3 targetClumpAverage = Vector3.zero;
          foreach (var targ in m_cleanTargetList)
          {
            targetClumpAverage += targ.position;
          }
          targetClumpAverage /= m_cleanTargetList.Count;

          //Get the distance of the clump to the primary target
          Vector3 clumpDistanceToPrimary = targetClumpAverage - m_primaryTarget.position;

          //Look somewhere between the primary target and the average secondary target group position
          m_cameraDesiredLookPoint = m_primaryTarget.position + clumpDistanceToPrimary * m_clumpBias;

          //Find the futhest target from the centre of combat
          float furthestTargetDistance = (m_cameraDesiredLookPoint - m_primaryTarget.position).magnitude;
          foreach(var targ in m_cleanTargetList)
          {
            float targDist = (m_cameraDesiredLookPoint - targ.position).magnitude;
            if(targDist > furthestTargetDistance)
              furthestTargetDistance = targDist;
          }

          //The furthest valid target will be used to scale the camera closer or further from combat
          float distanceScaler = furthestTargetDistance * m_groupDistanceScaling;
          if(distanceScaler < 1)
            distanceScaler = 1;

          m_cameraDesiredPosition = m_cameraDesiredLookPoint + new Vector3(
              Mathf.Cos(m_viewAngle) * m_relativePosition.x * distanceScaler, //Move back from that centre point given the current angle, distance between the targets & any added space
              m_relativePosition.y, //just moves it up
              Mathf.Sin(m_viewAngle) * m_relativePosition.x * distanceScaler); //Same as the other

          break;
        case CameraStates.LockOn:
          //code here
          break;
      }
 
      //Apply the given position and rotation to the camera
      transform.position = Vector3.Lerp(transform.position, m_cameraDesiredPosition, Time.deltaTime * m_cameraMovementSpeed);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_cameraDesiredLookPoint - transform.position), Time.deltaTime * m_cameraRotationSpeed);
      //Run the auto rotation check
      AutoRotateCamera();
      //If the player recently gave an input - update the camera's rotation
      if(m_cameraMoveCountdown > 0)
        SetDesiredAngle(m_playerCameraRotationInput);
      //Lerp the target angle to the desired on for nice smooth camera rotation
      m_viewAngle = Mathf.Lerp(m_viewAngle, m_desiredAngle, (m_cameraMoveCountdown <= 0 ? m_cameraAutoPivotSpeed : m_cameraPivotSpeed)* Time.deltaTime * (1 + Mathf.Abs(m_desiredAngle - m_viewAngle) * m_pivotGapStrength));
    }

    public void SetDesiredAngle(float value, bool additive = true)
    {
      if(additive)
        m_desiredAngle += value;
      else
      {
        //Take the given input and change it to a delta-like value (IDK how this works but it does)
        m_desiredAngle += value - m_desiredAngle;
      }
      
      //This value needs to remain between 0 & 2Pi as its a radian
      if(m_desiredAngle < 0)
      {
        m_desiredAngle += Mathf.PI * 2;
      }
      if(m_desiredAngle > Mathf.PI * 2)
      {
        m_desiredAngle -= Mathf.PI * 2;
      }
      //Check if the difference in the desired and current angle is greater then 180
      float angleDiff = m_desiredAngle - m_viewAngle;
      if(Mathf.Abs(angleDiff) > Mathf.PI)
      {
        //Apply a 360 rotation to the current angle - this will cause it to spin the other way
        if(angleDiff > 0)
          m_viewAngle += Mathf.PI * 2;
        else if (angleDiff < 0)
          m_viewAngle -= Mathf.PI * 2;
      }
    }

    private void AutoRotateCamera()
    {
      //Check if the countdown has been reached
      if(m_cameraMoveCountdown <= 0)
      {
        //Check the state
        switch(m_camState)
        {
          case CameraStates.Explore: //Move behind the player
            float primTargetAngle = m_primaryTarget.rotation.eulerAngles.y * Mathf.Deg2Rad;
            SetDesiredAngle(-primTargetAngle + Mathf.PI * 1.5f, false);
            break;
          case CameraStates.Combat: //Move alongside the combat
            SetDesiredAngle(Vector3.Angle((m_primaryTarget.position - m_cameraDesiredLookPoint).normalized, -Vector3.forward) * Mathf.Deg2Rad, false);
            break;
        }
      }
      else //Reduce the countdown value
      {
        m_cameraMoveCountdown -= Time.deltaTime;
      }
    }

    public void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.green;
      Gizmos.DrawSphere(m_cameraDesiredPosition, 0.25f);
      Gizmos.DrawSphere(m_primaryTarget.position, 0.15f);
      Gizmos.DrawWireSphere(m_primaryTarget.position, m_combatRangeCutoff);

      Gizmos.color = Color.yellow;
      Gizmos.DrawSphere(m_cameraDesiredLookPoint, 0.25f);

      foreach(var targ in m_secondaryTargets)
      {
        if(m_cleanTargetList.Contains(targ))
          Gizmos.color = Color.green;
        else
          Gizmos.color = Color.red;

        Gizmos.DrawSphere(targ.position, 0.5f);
      }
    }

    public void AssignTarget(Transform targetObject, bool newPrimary = false)
    {
      //If this new target is to be the primary and isn't already
      if(newPrimary && m_primaryTarget != targetObject)
      {
        //Make it the primary target
        m_primaryTarget = targetObject;
      }
      //If this new target isn't already a target
      else if(!m_secondaryTargets.Contains(targetObject) && m_primaryTarget != targetObject)
      {
         //create a new tmp array and recreate the main one plus 1 index
        Transform[] tmp = new Transform[m_secondaryTargets.Length + 1];
        for(int i = 0; i < m_secondaryTargets.Length; i++)
        {
          tmp[i] = m_secondaryTargets[i];
        }
        tmp[tmp.Length - 1] = targetObject;
        m_secondaryTargets = tmp;
      }
    }
    public void RemoveTarget(int arrayIndex)
    {
      m_secondaryTargets[arrayIndex] = null;
      Transform[] tmp = new Transform[m_secondaryTargets.Length - 1];
      for(int i = 0; i < m_secondaryTargets.Length; i++)
      {
        if(i < arrayIndex)
          tmp[i] = m_secondaryTargets[i];
        else if (i > arrayIndex)
          tmp[i - 1] = m_secondaryTargets[i];
      }
      m_secondaryTargets = tmp;
    }
    public void RemoveTarget(Transform targetObject)
    {
      if(m_secondaryTargets.Contains(targetObject))
      {
        for(int i = 0; i < m_secondaryTargets.Length; i++)
        {
          if(m_secondaryTargets[i] == targetObject)
          {
            RemoveTarget(i);
          }
        }
      }
    }
    public void OnLook(Vector2 value)
    {
      //add the delta of the player input to the desired angle.
      m_cameraMoveCountdown = m_cameraMoveWait;
      m_playerCameraRotationInput = Mathf.Clamp(value.x, -1, 1) * m_inputSensitivity * Time.deltaTime;
    }

    public void ChangeState(CameraStates newState)
    {
      if(m_camState != newState)
      {
        m_camState = newState;
      }
    }
}}
