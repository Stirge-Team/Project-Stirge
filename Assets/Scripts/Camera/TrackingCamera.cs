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

    private Vector3 targetPosition;
    private Vector3 targetLookPosition;
    private Vector3 targetDistance; 
    //private Transform m_cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      //Calc the position the camera should look at
      Vector3 targetLookPosition = Vector3.zero;
      targetPosition = Vector3.zero;
      float distanceScaler = 1;
      //If there is no other target just look at the primary
      //(if there are no targets the camera will just use the last known targetPosition)
      if(!m_secondaryTarget)
      {
        targetLookPosition = m_primaryTarget.position;
      }
      //If there are 2 targets...
      else if (m_primaryTarget && m_secondaryTarget)
      {
        //Get the distance from the primary target to the secondary
        targetDistance = m_secondaryTarget.position - m_primaryTarget.position;

        //Get the target position at the distance between the targets
        targetLookPosition = m_primaryTarget.position + targetDistance / 2;
        //if the primary target is above the secondary target
        if(m_primaryTarget.position.y > m_secondaryTarget.position.y)
          //set the Y to the primary target's
          targetLookPosition = new Vector3(targetLookPosition.x, m_primaryTarget.position.y, targetLookPosition.z);

        //Get the distance scaler to adjust how far the camera will be from the targets
        distanceScaler = targetDistance.magnitude;
        //Make sure its at least 1
        if(distanceScaler < 1)
          distanceScaler = 1;

        //Start with the look position - thus it becomes the centre
        targetPosition = new Vector3(targetLookPosition.x, targetLookPosition.y, targetLookPosition.z) + new Vector3(
            Mathf.Cos(m_relativeAngle) * (distanceScaler/2) * m_relativePosition.x, //Move back from that centre point given the current angle, distance between the targets & any added space
            m_relativePosition.y, //just moves it up
            Mathf.Sin(m_relativeAngle) * (distanceScaler/2) * m_relativePosition.x); //Same as the other

        /*{
        

        //Reduce it by the height the camera will be
        //distanceScaler -= Mathf.Abs(m_relativePosition.y);



        //Get target distances from the targetPosition;
        Vector3 distanceToPrimary = new Vector3(m_primaryTarget.position.x, 0, m_primaryTarget.position.z) - new Vector3(targetPosition.x, 0, targetPosition.z);
        Vector3 distanceToSecondary = new Vector3(m_secondaryTarget.position.x, 0, m_secondaryTarget.position.z) - new Vector3(targetPosition.x, 0, targetPosition.z);
        
        if(Mathf.Abs(distanceToPrimary.magnitude) < m_targetAvoidanceDistance)
        //while (distanceToPrimary.magnitude < m_targetAvoidanceDistance)
        {
          Debug.Log("Too close to primary target. " + Mathf.Abs(distanceToPrimary.magnitude));
          targetPosition -= distanceToPrimary / 2;
        }
        else if(Mathf.Abs(distanceToSecondary.magnitude) < m_targetAvoidanceDistance)
        //while (distanceToSecondary.magnitude < m_targetAvoidanceDistance)
        {
          Debug.Log("Too close to secondary target. " + Mathf.Abs(distanceToSecondary.magnitude));
          targetPosition -= distanceToSecondary / 2;
        }

        //Debug.Log($"Camera Report:\nDistance between targets: {targetDistance} | {distanceScaler} | Target position: {targetPosition}");
      }*/
      }
      //You won't believe what this one does
      transform.LookAt(targetLookPosition);
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

    public void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.green;
      Gizmos.DrawSphere(targetPosition, 0.25f);

      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(m_primaryTarget.position, m_targetAvoidanceDistance);
      Gizmos.DrawWireSphere(m_secondaryTarget.position, m_targetAvoidanceDistance);

      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(m_primaryTarget.position + targetDistance / 2, m_relativePosition.x);
    }
}}
