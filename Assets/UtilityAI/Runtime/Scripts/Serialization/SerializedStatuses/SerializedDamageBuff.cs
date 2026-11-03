using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;

    [NameOverride("Modifier Type", 0), NameOverride("Modifier", 1)]
    [CreateAssetMenu(menuName = "Utility AI/Serialized Statuses/Damage Buff", fileName = "New Damage Buff")]
    public class SerializedDamageBuff : SerializedStatus<DamageBuff, ModifierType, float>
    {
        
    }
}
