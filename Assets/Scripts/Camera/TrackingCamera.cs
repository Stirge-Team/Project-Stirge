using UnityEngine;

namespace Strige.Camera {
public class TrackingCamera : MonoBehaviour
{
    //camera needs to track between 2 objects
    //one object will be the primary target (the player 99%) - used for the position of the camera
    //the other will be the secondary target - used for the math on where the camera points
    //these objects should be able to be set by other scripts (maybe in editor but def in scripts)
    //speed, relative position are variables (idk ill figure it out as i go trust me bro)
    //
    [Header("Position and Movement")]
    [SerializeField, Tooltip("The speed at which the camera will move to it's desiered position")]
    private float m_cameraLerpSpeed;
    [SerializeField, Tooltip("The position relative to the look position that the camera should be")]
    private Vector2 m_relativePosition;
    [SerializeField, Tooltip("The angle around the target position that the camera should base it's relative position around")]
    private float m_relativeAngle;
    [SerializeField]
    private float m_targetAvoidanceDistance;
    [Header("Targets")]
    [SerializeField, Tooltip("The primary target for the camera - used for position and as the default object to look at")]
    private Transform m_primaryTarget;
    [SerializeField, Tooltip("The secondary target for the camera - used for the math on where the camera points")]
    private Transform m_secondaryTarget;
    //private Transform m_cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      UpdateRelativeAngle(0.025f);
      //Calc the position the camera should look at
      Vector3 targetLookDir = Vector3.zero;
      Vector3 targetPosition = Vector3.zero;
      float distanceScaler = 1;
      //If there is no other target just look at the primary
      //(if there are no targets the camera will just use the last known targetPosition)
      if(!m_secondaryTarget)
      {
        targetLookDir = m_primaryTarget.position;
      }
      //If there are 2 targets...
      else if (m_primaryTarget && m_secondaryTarget)
      {
        //Get their distance
        Vector3 targetDistance = m_secondaryTarget.position - m_primaryTarget.position;
        
        //Get the distance scaler to adjust how far the camera will be from the targets
        distanceScaler = targetDistance.magnitude;
        //Reduce it by the height the camera will be
        distanceScaler -= Mathf.Abs(m_relativePosition.y);
        //Make sure its at least 1
        if(distanceScaler < 1)
          distanceScaler = 1;

        //Get the target position at the distance between the targets
        targetLookDir = m_primaryTarget.position + targetDistance / 2;
        float topY = 0;
        if(m_primaryTarget.position.y > m_secondaryTarget.position.y)
          topY = m_primaryTarget.position.y;
        else
          topY = targetLookDir.y;

        targetPosition = new Vector3(targetLookDir.x, topY, targetLookDir.z) + new Vector3(
            Mathf.Cos(m_relativeAngle) * m_relativePosition.x, 
            m_relativePosition.y, 
            Mathf.Sin(m_relativeAngle) * m_relativePosition.x);
        
        /*
        Vector3 distanceToPrimary = m_primaryTarget.position - targetPosition;
        Vector3 distanceToSecondary = m_secondaryTarget.position - targetPosition;
        Debug.Log($"Target distances: PRM-{Mathf.Abs(distanceToPrimary.magnitude)} SND-{Mathf.Abs(distanceToSecondary.magnitude)}");

        if(Mathf.Abs(distanceToPrimary.magnitude) < m_targetAvoidanceDistance)
          //while (distanceToPrimary.magnitude < m_targetAvoidanceDistance)
          {
            Debug.Log("Too close to primary target. " + Mathf.Abs(distanceToPrimary.magnitude));
            targetPosition += distanceToPrimary * 0.2f;
            distanceToPrimary = m_primaryTarget.position - targetPosition;
            Debug.Log("New position: " + targetPosition);
          }
        if(Mathf.Abs(distanceToSecondary.magnitude) < m_targetAvoidanceDistance)
          //while (distanceToSecondary.magnitude < m_targetAvoidanceDistance)
          {
            Debug.Log("Too close to secondary target. " + Mathf.Abs(distanceToSecondary.magnitude));
            targetPosition += distanceToSecondary * 0.2f;
            distanceToSecondary = m_secondaryTarget.position - targetPosition;
            Debug.Log("New position: " + targetPosition);
          }
          */

        Debug.Log($"Camera Report:\nDistance between targets: {targetDistance} | {distanceScaler} | Target position: {targetPosition}");
      }
      //You won't believe what this one does
      transform.LookAt(targetLookDir);
      //Calc the relative position of the camera then add that to the target position and apply
      transform.position = targetPosition;
    }

    public void UpdateRelativeAngle(float value)
    {
      m_relativeAngle += value;
      
      //This value needs to remain between 0 & 2Pi as its a radian
      if(m_relativeAngle < 0)
      {
        m_relativeAngle += Mathf.PI * 2;
      }
      else if(m_relativeAngle > Mathf.PI * 2)
      {
        m_relativeAngle -= Mathf.PI * 2;
      }
    }
}}
