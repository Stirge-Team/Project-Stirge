using UnityEngine;

namespace Stirge.InfiniteAxis.Demo
{
    public enum ResourceType
    {
        Log,
        Food
    }

    public class DemoResource : MonoBehaviour
    {
        [SerializeField] private ResourceType m_resourceType;

        public bool IsTypeOfResource(ResourceType type)
        {
            return m_resourceType == type;
        }
    }
}