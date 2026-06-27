using UnityEngine;

namespace Stirge
{
    public class DestructableObject : MonoBehaviour
    {
        //public GameObject m_brokenStateObject;
        public float m_forceThreashhold;
        public float m_impactExaggeration = 1;
        public ParticleSystem m_dustParticles;
        void Start()
        {
            SetChildKinematics(true);
        }

        public void OnTriggerEnter(Collider incommingCollision)
        {
            if(incommingCollision.transform.IsChildOf(transform)) return; //exit if child object


            Rigidbody incommingRigidbody = incommingCollision.gameObject.GetComponent<Rigidbody>(); //get the incoming rigidbody

            if (!incommingRigidbody) //no rb?
                incommingRigidbody = incommingCollision.gameObject.GetComponentInChildren<Rigidbody>(); //look for one in the children


            if (incommingRigidbody) //rb found!
            {
                var impactForce = 0.5f * incommingRigidbody.mass * incommingRigidbody.linearVelocity.sqrMagnitude;
                if (impactForce >= m_forceThreashhold) //are they moving fast enough to break this object?
                {
                    //Debug.DrawRay(imcommingRigidbody.transform.position, imcommingRigidbody.linearVelocity, Color.red, 3);
                    SetChildKinematics(incommingRigidbody.linearVelocity * impactForce * m_impactExaggeration, incommingRigidbody.transform.position);

                    if (m_dustParticles)
                    {
                        //spawn particles :)
                        ParticleSystem particleInstance = Instantiate(m_dustParticles, transform.position, Quaternion.identity);
                        //particleInstance.shape.rotation = Quaternion.LookRotation(rigidbody.linearVelocity).eulerAngles;
                        particleInstance.Play();
                    }

                    gameObject.GetComponent<Collider>().enabled = false; //disable collider
                }
            }
        }

        private void SetChildKinematics(bool setValue)
        {
            foreach (var child in GetComponentsInChildren<Rigidbody>())
            {
                child.isKinematic = setValue;
            }
        }
        private void SetChildKinematics(Vector3 force, Vector3 position)
        {
            foreach(var child in GetComponentsInChildren<Rigidbody>())
            {
                child.isKinematic = false;
                child.transform.SetParent(null);
                child.AddForceAtPosition(force, position, ForceMode.Impulse);
            }
        }
    }
}
