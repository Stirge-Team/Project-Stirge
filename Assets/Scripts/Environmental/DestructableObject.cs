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

        public void OnTriggerEnter(Collider collision)
        {
            if(collision.transform.IsChildOf(transform)) return; //exit if child object


            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>(); //get the incoming rigidbody

            if (!rigidbody) //no rb?
                rigidbody = collision.gameObject.GetComponentInChildren<Rigidbody>(); //look for one in the children


            if (rigidbody) //rb found!
            {
                if (rigidbody.linearVelocity.sqrMagnitude >= m_forceThreashhold) //are they moving fast enough to break this object?
                {
                    var direction = -(rigidbody.transform.position - transform.position).normalized;
                    Debug.DrawRay(rigidbody.transform.position, rigidbody.linearVelocity, Color.red, 3);
                    SetChildKinematics(direction * rigidbody.linearVelocity.sqrMagnitude * m_impactExaggeration, rigidbody.transform.position);

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
                child.AddForceAtPosition(force, position, ForceMode.Impulse);
            }
        }
    }
}
