using System;
using UnityEditor.Search;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour
{
    [Serializable]
    public struct ParticleRef
    {
        public string Name => m_name;
        [SerializeField, Tooltip("The name of this particle effect (for referencing)")]
        private string m_name;
        [SerializeField, Tooltip("The particle effect you want to play.")]
        private ParticleSystem m_particleEffect;
        [SerializeField, Tooltip("Where the above particle should normal be spawned in.")]
        private Transform m_defaultLocation;
        public void PlayParticle(Transform location = null)
        {
            ParticleSystem newInst = Instantiate(m_particleEffect);
            newInst.transform.SetParent(location != null ? location : m_defaultLocation, false);
            newInst.Play();
        }
    }
    [SerializeField]
    private ParticleRef[] m_particles;

    public void PlayParticle(string effectName, Transform location = null)
    {
        int index = -1;
        try
        {
            index = Convert.ToInt32(effectName);
            if (index >= m_particles.Length)
                throw new IndexOutOfRangeException("The given index is outside of the particle instancer array!");
        }
        catch (Exception e)
        {
            Debug.Log("Given string is NOT a valid int. Continuing as normal...\n" + e);
        }

        for (int i = index >= 0 ? index : 0; i < m_particles.Length; i++)
        {
            if (m_particles[i].Name == effectName || index == i)
            {
                m_particles[i].PlayParticle(location);
                return;
            }
        }
        Debug.LogError($"No particle found with name {effectName}. Please make sure you're calling the correct paricle instancer and that your strings are matching.");
    }

    //this function is here to faciliate unityevents being very very limited in the parameters they pass which is stupid and dumb, let me pass 30000 params if that is what i please.
    public void PlayParticle(string effectName)
    {
        PlayParticle(effectName, null);
    }
}
