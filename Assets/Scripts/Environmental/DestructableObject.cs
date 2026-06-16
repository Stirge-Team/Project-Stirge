using UnityEngine;

namespace Stirge
{
    public class DestructableObject : MonoBehaviour
    {
        public GameObject m_brokenStateObject;
        public float m_forceThreashhold;
        public float m_impactExaggeration = 1;
        public ParticleSystem m_dustParticles;

        public void OnCollisionEnter(Collision collision)
        {

            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>(); //get the incoming rigidbody

            if (!rigidbody) //no rb?
                rigidbody = collision.gameObject.GetComponentInChildren<Rigidbody>(); //look for one in the children


            if (rigidbody) //rb found!
            {
                if (rigidbody.linearVelocity.sqrMagnitude >= m_forceThreashhold) //are they moving fast enough to break this object?
                {
                    Debug.DrawRay(rigidbody.transform.position, rigidbody.linearVelocity, Color.red, 3);
                    if (m_brokenStateObject)
                    {
                        GameObject debris = Instantiate(m_brokenStateObject, transform.position, Quaternion.identity); //spawn debris
                        debris.transform.localScale = transform.localScale;

                        //apply force to it and any child objects!
                        Rigidbody debrisRB = debris.GetComponent<Rigidbody>();
                        if (debrisRB)
                        {
                            debrisRB.AddForceAtPosition(-rigidbody.linearVelocity * m_impactExaggeration, collision.transform.position, ForceMode.Impulse);
                            Debug.DrawRay(debris.transform.position, -rigidbody.linearVelocity * m_impactExaggeration, Color.blue, 3);
                        }
                        foreach (var childRB in debris.GetComponentsInChildren<Rigidbody>())
                        {
                            childRB.AddForceAtPosition(-rigidbody.linearVelocity * m_impactExaggeration, collision.transform.position, ForceMode.Impulse);
                            Debug.DrawRay(childRB.transform.position, -rigidbody.linearVelocity * m_impactExaggeration, Color.blue, 3);
                        }
                    }

                    if (m_dustParticles)
                    {
                        //spawn particles :)
                        ParticleSystem particleInstance = Instantiate(m_dustParticles, transform.position, Quaternion.identity);
                        //particleInstance.shape.rotation = Quaternion.LookRotation(rigidbody.linearVelocity).eulerAngles;
                        particleInstance.Play();
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}
