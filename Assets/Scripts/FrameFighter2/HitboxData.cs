using Stirge.Combat;
using UnityEngine;

namespace FrameFighter2.Data
{
    [System.Serializable]
    public class HitboxData
    {
        /// <summary>
        /// Determines the shape of the hitbox
        /// </summary>
        public enum HitboxShapes
        {
            Rectangle,
            Capsule,
            Sphere,
        }
        /// <summary>
        /// Determines if the hitbox should have a parent or not
        /// </summary>
        public enum HitboxTypes
        {
            Local, //Spawns hitbox as a child of the frame data manager
            World, //Spawns hitbox with no parent
            AttachedToBone, //Spawns hitbox parented to specified object
        }

        //position, scale, and rotation
        [SerializeField] private Vector3 m_position;
        [SerializeField] private Vector3 m_rotation;
        [SerializeField] private Vector3 m_scale;
        //start and end frame
        [SerializeField] private float m_startFrame;
        [SerializeField] private float m_endFrame;
        //group ID
        [SerializeField] private int m_groupID;
        //Shape of hitbox
        [SerializeField] private HitboxShapes m_hitboxShape;
        //Hitbox Type and parent object
        [SerializeField] private HitboxTypes m_hitboxType;
        [SerializeField] private string m_hitboxParent;
        //event on hitbox
        [SerializeField] private EventData m_onHitEvent;
        //OnHitEffect
        [SerializeField] private OnHitEffect m_onHitEffect;

        public Vector3 Position => m_position;
        public Vector3 Rotation => m_rotation;
        public Vector3 Scale => m_scale;
        public float StartFrame => m_startFrame;
        public float EndFrame => m_endFrame;
        public int GroupID => m_groupID;
        public HitboxShapes HitboxShape => m_hitboxShape;
        public HitboxTypes HitboxType => m_hitboxType;
        public string HitboxParent => m_hitboxParent;
        public EventData OnHitEvent => m_onHitEvent;
        public OnHitEffect OnHitEffect => m_onHitEffect;

        public HitboxData(Vector3 position, Vector3 rotation, Vector3 scale, float startFrame, float endFrame, 
            int groupID = 0, HitboxShapes hitboxShape = HitboxShapes.Rectangle, HitboxTypes hitboxType = HitboxTypes.Local, 
            string hitboxParent = "", EventData onHit = null, OnHitEffect onHitEffect = null)
        {
            m_position = position;
            m_rotation = rotation;
            m_scale = scale;
            m_startFrame = startFrame;
            m_endFrame = endFrame;
            m_groupID = groupID;
            m_hitboxShape = hitboxShape;
            m_hitboxType = hitboxType;
            m_hitboxParent = hitboxParent;
            m_onHitEvent = onHit;
            m_onHitEffect = onHitEffect;
        }
    }
}