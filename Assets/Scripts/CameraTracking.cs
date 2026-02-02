using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform m_trackedTarget;
    [SerializeField] private float m_cameraRotationSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 relativePos = m_trackedTarget.position - transform.position;
      transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), m_cameraRotationSpeed);
    }
}
