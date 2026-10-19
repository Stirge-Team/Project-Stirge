using UnityEngine;
using UnityEngine.AI;

namespace Stirge.Tools
{
    public static class NavMeshAgentHelper
    {
        public static bool HasAgentReachedDestination(NavMeshAgent agent)
        {
            // Check if we've reached the destination
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
