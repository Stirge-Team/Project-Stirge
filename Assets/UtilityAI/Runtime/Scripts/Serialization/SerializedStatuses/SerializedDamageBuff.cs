using Stirge.Combat;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Serialization;

    [NameOverride("Modifier Type", 0), NameOverride("Modifier", 1)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Statuses/Damage Buff", fileName = "New Damage Buff", order = 451)]
    public class SerializedDamageBuff : SerializedStatus<DamageBuff, ModifierType, float>
    {
        
    }
}
