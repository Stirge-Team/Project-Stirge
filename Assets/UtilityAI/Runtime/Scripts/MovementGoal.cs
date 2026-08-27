using Stirge.Combat;
using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class MovementGoal : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] private float m_scoreScaling = 1f;
        [Tooltip("The length of time this Goal will be performed for until the Enemy attempts to re-evaluate its Movement Goals.")]
        [SerializeField, Min(0f)] private float m_duration;

        public float Evaluate(CombatEntity user)
        {
            float baseScore = EvaluateInternal(user);
            return baseScore * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(CombatEntity user);

        #region Setup
        public static TMovementGoal Create<TMovementGoal>() where TMovementGoal : MovementGoal, INotSetupable, new()
        {
            var movementGoal = new TMovementGoal();
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg>(TArg arg) where TMovementGoal : MovementGoal, ISetupable<TArg>, new()
        {
            var movementGoal = new TMovementGoal();
            movementGoal.Setup(arg);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ0>(TArg0 arg0, Targ0 arg1) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ0>, new()
        {
            var movementGoal = new TMovementGoal();
            movementGoal.Setup(arg0, arg1);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var movementGoal = new TMovementGoal();
            movementGoal.Setup(arg0, arg1, arg2);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var movementGoal = new TMovementGoal();
            movementGoal.Setup(arg0, arg1, arg2, arg3);
            return movementGoal;
        }
        public static TMovementGoal Create<TMovementGoal, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4) where TMovementGoal : MovementGoal, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var movementGoal = new TMovementGoal();
            movementGoal.Setup(arg0, arg1, arg2, arg3, arg4);
            return movementGoal;
        }
        #endregion
    }
}
