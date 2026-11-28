using Stirge.Player;
using UnityEngine;

namespace Stirge.Player
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        private float m_transparency = 0.5f;
        void Start()
        {
            Player player = FindAnyObjectByType<Player>();
            player.Motor.SetPositionAndRotation(transform.position, transform.rotation);
        }

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new(1f, 0f, 0f, 1f - m_transparency);
            Gizmos.DrawCube(Vector3.zero, new(1f, 2f, 1f));

            UnityEditor.Handles.matrix = transform.localToWorldMatrix;
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.identity, 1f, EventType.Repaint);
            //Gizmos.DrawLine(Vector3.zero, Vector3.forward);
        }
        #endif
    }
}
