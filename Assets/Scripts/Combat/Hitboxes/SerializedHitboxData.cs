using Stirge.Camera;
using Stirge.Sound;
using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    [System.Serializable]
    public struct SerializedHitboxData
    {
        [Tooltip("The Mask the HitboxCollider will use to check for Hittables.")]
        [SerializeField] private LayerMask m_collisionMask;
        [Tooltip("The damage that will be dealt to the Hittable when hit by the HitboxCollider.")]
        [SerializeField] private int m_damage;
        [Tooltip("The camera shake settings to apply when this move connects.")] 
        [SerializeField] private CameraShakePreset m_cameraShakePreset;
        [Tooltip("How long this move's hitstop should last.")]
        [SerializeField, Min(0)] private float m_hitstopDuration;
        [Tooltip("How strong this move's hitstop should affect time scale be")]
        [SerializeField, Range(0f,1f)] private float m_hitstopScale;
        [Tooltip("A sound to be played when this attack lands.")]
        [SerializeField] private SoundClip m_hitSound;
        [Tooltip("Details for the Statuses to inflict.")]
        [SerializeField] private StatusInflictData[] m_statusInflictData;
        
        public readonly LayerMask CollisionMask => m_collisionMask;
        public readonly int Damage => m_damage;
        public readonly CameraShakePreset CameraShakePreset => m_cameraShakePreset;
        public readonly float HitstopDuration => m_hitstopDuration;
        public readonly float HitstopScale => m_hitstopScale;
        public readonly SoundClip HitSound => m_hitSound;
        public readonly StatusInflictData[] StatusInflictDatas => m_statusInflictData;
    }
}
