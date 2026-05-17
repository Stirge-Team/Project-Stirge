using System;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour
{
    [Serializable]
    public struct ParticleRef
    {
        public string Name => m_name;
        [SerializeField]
        private string m_name;
        [SerializeField]
        private ParticleSystem m_particleEffect;
        [SerializeField]
        private Transform m_defaultLocation;
        public void PlayParticle(Transform location)
        {
            ParticleSystem newInst = Instantiate(m_particleEffect);
            newInst.transform.SetParent(location, false);
            newInst.Play();
        }
        public void PlayParticle()
        {
            PlayParticle(m_defaultLocation);
        }
    }
    [SerializeField]
    private ParticleRef[] m_particles;

    public void PlayParticle(string effectName, Transform location = null)
    {
        foreach (var effect in m_particles)
        {
            if (effect.Name == effectName)
            {
                if (!location)
                    effect.PlayParticle();
                else
                    effect.PlayParticle(location);
            }
        }
    }

    //this function is here to faciliate unityevents being very very limited in the parameters they pass which is stupid and dumb, let me pass 30000 params if that is what i please.
    public void PlayParticle(string effectName)
    {
        PlayParticle(effectName, null);
    }
}
