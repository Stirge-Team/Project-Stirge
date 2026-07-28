using UnityEngine;
using System.Collections.Generic;
using Stirge.Serialization;
using System;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public abstract class AttackNode
    {
        public string name { get; set; }
        
        /// <summary>
        /// Either adds itself to the <paramref name="activeNodes"/> list or adds other AttackNodes.<br/>
        /// Because <see cref="List{T}"/> is a reference type, adding to <paramref name="activeNodes"/> actually adds it to the list,
        /// where it is used to create the Attack Sequence after this method is finished being performed.
        /// </summary>
        /// <param name="activeNodes">The list of active nodes.</param>
        public abstract void Evaluate(List<AttackNode> activeNodes);

        public static readonly Type[] AttackNodeTypes =
        {
            typeof(AnimationNode),
            typeof(ApproachTargetNode),
            typeof(SelectAttackNode),
            typeof(SequenceAttackNode),
            typeof(TranslateNode),
            typeof(ChanceNode),
            typeof(DelayNode),
            typeof(SimultaneousAttackNode),
            typeof(TimedMoveNode),
            typeof(CurveMoveNode),
            typeof(SpeedMoveNode),
            typeof(AccelerateMoveNode),
        };

        #region Initialisers
        public static TAttackNode Create<TAttackNode>() where TAttackNode : AttackNode, INotSetupable, new()
        {
            var node = new TAttackNode();
            return node;
        }
        public static TAttackNode Create<TAttackNode, TArg>(TArg arg) where TAttackNode : AttackNode, ISetupable<TArg>, new()
        {
            var node = new TAttackNode();
            node.Setup(arg);
            return node;
        }
        public static TAttackNode Create<TAttackNode, TArg0, TArg1>(TArg0 arg0, TArg1 arg1) where TAttackNode : AttackNode, ISetupable<TArg0, TArg1>, new()
        {
            var node = new TAttackNode();
            node.Setup(arg0, arg1);
            return node;
        }
        public static TAttackNode Create<TAttackNode, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2) where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            var node = new TAttackNode();
            node.Setup(arg0, arg1, arg2);
            return node;
        }
        public static TAttackNode Create<TAttackNode, TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3) where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
        {
            var node = new TAttackNode();
            node.Setup(arg0, arg1, arg2, arg3);
            return node;
        }
        public static TAttackNode Create<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
        {
            var node = new TAttackNode();
            node.Setup(arg0, arg1, arg2, arg3, arg4);
            return node;
        }

        public static AttackNode Create(Type attackNodeType)
        {
            var node = (AttackNode)Activator.CreateInstance(attackNodeType);
            return node;
        }
        public static AttackNode Create(Type attackNodeType, object[] parameters)
        {
            var node = (AttackNode)Activator.CreateInstance(attackNodeType);
            SetuableHelper.CreateSetup(node, parameters);
            return node;
        }
        #endregion
    }
}
