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

    [Header("Position and Movement")]
    [SerializeField, Tooltip("The speed at which the camera will move to it's desiered position")]
    private float m_cameraMovementSpeed = 1;
    [SerializeField, Tooltip("The speed at which the camera will rotate to face it's target")]
    private float m_cameraRotationSpeed = 60;
    [SerializeField, Tooltip("The position relative to the look position that the camera should be")]
    private Vector2 m_relativePosition;
    private float m_viewAngle;

    [Header("Auto-Movement Settings")]
    [SerializeField, Tooltip("How long the game should respect the player's change to the camera position before attempting to move itself")]
    private float m_cameraMoveWait = 3;
    private float m_cameraMoveCountdown;
    [SerializeField, Tooltip("The speed at which the camera auto-pivots around it's origin")]
    private float m_cameraPivotSpeed = 1;

    [Header("Group Target Settings")]
    [SerializeField, Tooltip("How strong the distance scaling should be during group situations (multiplicative)")]
    private float m_groupDistanceScaling = 1;
    [SerializeField, Tooltip("How far until a target is out of range of the player in combat")]
    private float m_combatRangeCutoff;
    
    [Header("Targets")]
    [SerializeField, Tooltip("The primary target for the camera - used for position and as the default object to look at")]
    private Transform m_primaryTarget;
    [SerializeField, Tooltip("The secondary targets for the camera - used for the math on where the camera points. The 0th index of the array is for the lockon target")]
    private Transform[] m_secondaryTargets;


    private Vector3 m_cameraDesiredPosition;
    private Vector3 m_cameraDesiredLookPoint;
    private List<Transform> m_cleanTargetList = new();
    private float m_desiredAngle;

    private enum CameraStates
    {
        Explore,
        Combat,
        LockOn
    };
    [SerializeField]
    private CameraStates m_camState;

    // Update is called once per frame
    void Update()
    {
      switch (m_camState)
      {
        case CameraStates.Explore:
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

          //Find the distance from the camera the player is
          float primaryDistanceFromCamera = (new Vector3(m_primaryTarget.position.x - transform.position.x, 0, m_primaryTarget.position.z - transform.position.z)).magnitude;
          //If the camera cooldown has been reached - begin auto rotation
          if(m_cameraMoveCountdown <= 0)
          {
            //Begin auto rotating behind the player
            SetDesiredAngle((-m_primaryTarget.rotation.eulerAngles.y - 90) * Mathf.Deg2Rad);
          }
          else
            m_cameraMoveCountdown -= Time.deltaTime;

          //Take the primary target position and add the relative position values
            m_cameraDesiredPosition = m_primaryTarget.position + new Vector3(
              Mathf.Cos(m_viewAngle) * m_relativePosition.x, //Move back from that centre point given the current angle, distance between the targets & any added space
              m_relativePosition.y, //just moves it up
              Mathf.Sin(m_viewAngle) * m_relativePosition.x); //Same as the other
          //Get the distance from the desired position and primary target (ignoring the Y), and look in that direction, a bit past the target
          m_cameraDesiredLookPoint = m_primaryTarget.position + (m_primaryTarget.position - new Vector3(m_cameraDesiredPosition.x, m_primaryTarget.position.y, m_cameraDesiredPosition.z)).normalized * primaryDistanceFromCamera;
          break;
        case CameraStates.Combat:
          // - Keep surroundings, player & enemies on screen
          // - This and LO are gonna be similar
          // - Camera should adjust the number of enemies smoothly
          // - Account for multiple targets
          // - Camera should zoom to account for many foes
          // - Max and Min ranges for the zooming

          //Target check
          if(m_secondaryTargets.Length == 0)
          {
            m_camState = CameraStates.Explore;
            Debug.LogWarning("There are no combat targets for the camera to track... take your schizo pills (or check your code)");
            break;
          }
          m_cleanTargetList = new();
          //Check for any targets that are too far away
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

          //Get the average position of all valid targets
          Vector3 targetClumpAverage = Vector3.zero;
          foreach (var targ in m_cleanTargetList)
          {
            targetClumpAverage += targ.position;
          }
          targetClumpAverage /= m_cleanTargetList.Count;

          //Get the distance of the clump to the primary target
          Vector3 clumpDistanceToPrimary = targetClumpAverage - m_primaryTarget.position;

          //Look somewhere between the primary target and the average secondary target group position
          m_cameraDesiredLookPoint = m_primaryTarget.position + clumpDistanceToPrimary / 2;

          //Find the futhest target from the centre of combat
          float furthestTargetDistance = (m_cameraDesiredLookPoint - m_primaryTarget.position).magnitude;
          foreach(var targ in m_cleanTargetList)
          {
            float targDist = (m_cameraDesiredLookPoint - targ.position).magnitude;
            if(targDist > furthestTargetDistance)
              furthestTargetDistance = targDist;
          }

          float distanceScaler = furthestTargetDistance * m_groupDistanceScaling;
          if(distanceScaler < 1)
            distanceScaler = 1;

          //Check enough time has passed before attempting to move the camera around the pivot
          if(m_cameraMoveCountdown <= 0)
          {
            SetDesiredAngle(Vector3.Angle((m_primaryTarget.position - m_cameraDesiredLookPoint).normalized, -Vector3.forward) * Mathf.Deg2Rad);
          }
          else
            m_cameraMoveCountdown -= Time.deltaTime;

          m_cameraDesiredPosition = m_cameraDesiredLookPoint + new Vector3(
              Mathf.Cos(m_viewAngle) * m_relativePosition.x * distanceScaler, //Move back from that centre point given the current angle, distance between the targets & any added space
              m_relativePosition.y, //just moves it up
              Mathf.Sin(m_viewAngle) * m_relativePosition.x * distanceScaler); //Same as the other

          break;
        case CameraStates.LockOn:
          // Lock-ON (LAST)
          // - Frame the target and the player
          // - I'm picturing the pokemon combat screen positions for the player and the target
          // - Don't put the camera too far behind the player
          // - Keep the smooth movement

          //code here
          break;
      }

      //Lerp the target angle to the desired on for nice smooth camera rotation
      m_viewAngle = Mathf.Lerp(m_viewAngle, m_desiredAngle, m_cameraPivotSpeed * Time.deltaTime);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_cameraDesiredLookPoint - transform.position), Time.deltaTime * m_cameraRotationSpeed);
      transform.position = Vector3.Lerp(transform.position, m_cameraDesiredPosition, Time.deltaTime * m_cameraMovementSpeed);
    }

    public void SetDesiredAngle(float value, bool additive = false)
    {
      if(additive)
        m_desiredAngle += value;
      else
        m_desiredAngle = value;
      
      //This value needs to remain between 0 & 2Pi as its a radian
      while(m_desiredAngle < 0)
      {
        Debug.Log($"Correcting angle up"); 
        m_desiredAngle += Mathf.PI * 2;

      }
      while(m_desiredAngle > Mathf.PI * 2)
      {
        Debug.Log($"Correcting angle down");
        m_desiredAngle -= Mathf.PI * 2;

      }
      //This is to stop the camera whipping around, just adds or removes one rotation to the target angle rather then is having 
      if(m_desiredAngle - m_viewAngle > Mathf.PI * 1.5f){
        Debug.Log($"Large positive difference between the desired and current angle");
        m_viewAngle += Mathf.PI * 2;
      }
      else if(m_desiredAngle - m_viewAngle < -Mathf.PI * 1.5f){
        Debug.Log($"Large negative difference between the desired and current angle");
        m_viewAngle -= Mathf.PI * 2;
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
      if(newPrimary && m_primaryTarget != targetObject)
      {
        m_primaryTarget = targetObject;
      }
      else if(!m_secondaryTargets.Contains(targetObject) && m_primaryTarget != targetObject)
      {
        //create a new tmp array and recreate the main one plus 1 index
        Transform[] tmp = new Transform[m_secondaryTargets.Length + 1];
        for(int i = 0; i < m_secondaryTargets.Length; i++)
        {
          tmp[i] = m_secondaryTargets[i];
        }
        tmp[tmp.Length] = targetObject;
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
    public void OnLook(float value)
    {
      //add the delta of the player input to the desired angle.
      m_cameraMoveCountdown = m_cameraMoveWait;
      SetDesiredAngle(value, true);
    }
}}
