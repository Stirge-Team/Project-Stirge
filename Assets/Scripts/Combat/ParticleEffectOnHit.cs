using FrameFighter2.Hitbox;
using UnityEngine;

namespace Stirge.Combat
{
    [System.Serializable]
    public class ParticleEffectOnHit : Status
    {
        public ParticleEffectOnHit()
        {
            m_particleReference = null;
        }
        public ParticleEffectOnHit(ParticleSystem particle, Transform transform)
        {
            m_particleReference = particle;
        }

        [SerializeField]
        private ParticleSystem m_particleReference;

        public override void OnInflict(Transform hitboxTransform, CombatEntity targetEntity, CombatEntity attackingEntity)
        {
            ParticleSystem newInst = GameObject.Instantiate(m_particleReference, hitboxTransform.position, Quaternion.identity);
            newInst.transform.SetParent(hitboxTransform, false);
            newInst.transform.localPosition = Vector3.zero;
            newInst.Play();
        }
    }
}
