using Stirge.Serialization;
using Stirge.Tools;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [NameOverride("Translation", 0), NameOverride("Is Local Translation", 1), NameOverride("Time", 2)]
    public class SerializedTranslateNode : SerializedAttackNode<TranslateNode, Vector3, bool, RandomFloatField> { }
}
