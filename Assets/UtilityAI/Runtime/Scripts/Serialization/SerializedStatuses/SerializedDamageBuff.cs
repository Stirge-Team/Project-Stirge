using Stirge.Combat;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Serialization;

    [NameOverride("Modifier Type", 0), NameOverride("Modifier", 1), NameOverride("Duration", "Will fail if duration is 0 or less.", 2)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Statuses/Damage Buff", fileName = "New Serialized Damage Buff", order = 451)]
    public class SerializedDamageBuff : SerializedStatus<DamageBuff, ModifierType, float, float>
    {
        
    }
}
