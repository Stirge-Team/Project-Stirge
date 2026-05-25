using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public abstract class Behaviour
    {
        public virtual void _Enter(Agent agent) { }
        public abstract void _Update(Agent agent, float deltaTime);
        public virtual void _Exit(Agent agent) { }

        public static readonly System.Type[] BehaviourTypes =
        {
            typeof(AirJuggleBehaviour),
            typeof(AttackingBehaviour),
            typeof(PhysicsBehaviour),
            typeof(KnockbackBehaviour),
            typeof(LookAtTargetBehaviour),
            typeof(MoveToTargetBehaviour),
            typeof(UpdateTargetBehaviour),
        };
    }
}
