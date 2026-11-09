using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stirge.Destroyable
{
    public class Destroyable : MonoBehaviour
    {
        private Rigidbody m_rb;
        private Collider m_collider;
        private Renderer m_renderer;

        [Tooltip("How long in seconds it takes for fragments to fade away when completely still")][SerializeField] private float m_pieceSleepDelay = 1f;
        [Tooltip("How fast fragments fade away")][SerializeField] private float m_pieceFadeSpeed = 1f;

        [Tooltip("Selects random prefab to spawn when object is destroyed")][SerializeField] private GameObject[] m_brokenPrefabs;


        /// <summary>
        /// call to destroy object
        /// </summary>
        public void Destroy(Vector3 hitboxPosition, Vector3 hitboxVelocityVector)
        {
            //disable parts of rendering/interacting with object

            m_rb = GetComponent<Rigidbody>();

            if (m_rb != null)
            {
                Destroy(m_rb);
            }

            if (TryGetComponent<Collider>(out m_collider))
            {
                m_collider.enabled = false;
            }

            if (TryGetComponent<Renderer>(out m_renderer))
            {
                m_renderer.enabled = false;
            }

            //disable child objects

            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                Destroy(rb);
            }

            foreach (Renderer ren in GetComponentsInChildren<Renderer>())
            {
                ren.enabled = false;
            }

            //finish here if no prefabs added
            if (m_brokenPrefabs.Length == 0)
            {
                Destroy(this.gameObject);
                return;
            }

            //create destroyed version

            GameObject destroyedInstance = 
                Instantiate(m_brokenPrefabs[Random.Range(0, m_brokenPrefabs.Length)], transform.position, transform.rotation);

            Rigidbody[] fragmentRigidbodies = destroyedInstance.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody fragment in fragmentRigidbodies)
            {
                if (m_rb != null)
                {
                    //inherit current velocity
                    fragment.angularVelocity = m_rb.angularVelocity;
                    fragment.linearVelocity = m_rb.linearVelocity;

                    if (hitboxVelocityVector != Vector3.zero)
                    {
                        fragment.AddForceAtPosition(hitboxVelocityVector * 10f / Mathf.Clamp(Vector3.Distance(fragment.position, hitboxPosition), 1f, Mathf.Infinity),
                            hitboxPosition, ForceMode.Impulse);
                    }
                    //fallback if vector is equal to zero
                    else
                    {
                        fragment.AddExplosionForce(10f, hitboxPosition, 10f, 1f, ForceMode.Impulse);
                    }
                    //fragment.AddForce(hitboxVelocityVector * 50f / Mathf.Clamp(Vector3.Distance(fragment.position, hitboxPosition), 1f, Mathf.Infinity), ForceMode.Impulse);
                }
            }

            StartCoroutine(RemoveFragments(fragmentRigidbodies, destroyedInstance));
        }

        private IEnumerator RemoveFragments(Rigidbody[] fragmentRigidbodies, GameObject fragmentParent)
        {

            WaitForSeconds wait = new WaitForSeconds(m_pieceSleepDelay);
            int activeRigidbodies = fragmentRigidbodies.Length;
            bool[] isAsleep = new bool[activeRigidbodies];
            bool isAllAsleep = false;

            float sleepTimer = m_pieceSleepDelay;

            //wait untill all rigidbodies have fully stopped
            while (sleepTimer > 0)
            {
                yield return null;

                for (int i = 0; i <  activeRigidbodies; i++)
                {
                    isAsleep[i] = fragmentRigidbodies[i].IsSleeping();
                }

                isAllAsleep = Array.TrueForAll(isAsleep, element => element == true);

                //count down timer if no moving fragments, else reset timer
                sleepTimer = (isAllAsleep) ? sleepTimer - Time.deltaTime : m_pieceSleepDelay;
            }

            //fade out destroyed fragments

            //get renderers in all fragments
            float time = 0f;
            Renderer[] fragmentRenderers = Array.ConvertAll(fragmentRigidbodies, element => element.GetComponent<Renderer>());
            //destroy colliders and rigidbodies in fragments
            foreach(Rigidbody fragment in fragmentRigidbodies)
            {
                Destroy(fragment.GetComponent<Collider>());
                Destroy(fragment);
            }

            //get starting alpha of each fragment for proper linear interpolation
            float[] alpha = new float[fragmentRenderers.Length];
            for(int i = 0; i < alpha.Length; i++) 
            {
                alpha[i] = fragmentRenderers[i].material.color.a;
            }

            //fade out the fragments over time
            while (time < 1f)
            {
                time += Time.deltaTime * m_pieceFadeSpeed;
                for (int i = 0; i < fragmentRenderers.Length; i++)
                {
                    //set scale
                    //fragmentRenderers[i].transform.localScale = Vector3.Lerp(fragmentScale[i], Vector3.zero, time);

                    //set transparency
                    float newAlpha = Mathf.Lerp(alpha[i], 0f, time);
                    fragmentRenderers[i].material.color = new Color(fragmentRenderers[i].material.color.r, fragmentRenderers[i].material.color.g, fragmentRenderers[i].material.color.b, newAlpha);
                }

                yield return null;
            }

            //destroy all fragments
            foreach(Renderer fragmentRenderer in fragmentRenderers)
            {
                Destroy(fragmentRenderer.gameObject);
            }

            //destroy parent object and self
            Destroy(fragmentParent);
            Destroy(this.gameObject);
        }
    }
}

//leftover scale fadeout
//Vector3[] fragmentScale = new Vector3[fragmentRenderers.Length];
//for (int i = 0; i < fragmentScale.Length; i++)
//{
//    fragmentScale[i] = fragmentRenderers[i].transform.localScale;
//}


