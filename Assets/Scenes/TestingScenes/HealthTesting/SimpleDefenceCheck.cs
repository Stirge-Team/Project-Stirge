using Stirge.Management;
using UnityEngine;

public class SimpleDefenceCheck : MonoBehaviour
{
    public float defence;
    public void PassDefenceCheck(EntityHealth health, float amount, bool clamp, Object sender)
    {
        if(amount < 0)
            health.ModifyHealth(amount / defence);
        else
            health.ModifyHealth(amount);
    }
}
