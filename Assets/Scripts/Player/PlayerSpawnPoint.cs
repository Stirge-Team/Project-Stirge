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

        void OnDrawGizmos()
        {
            Color imlazy = new(0, 0, 0, m_transparency);
            Gizmos.color = Color.red - imlazy;
            Gizmos.DrawCube(transform.position, new(1, 2, 1));
            Gizmos.color += imlazy;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}
