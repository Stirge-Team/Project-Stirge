using Stirge.Management;
using UnityEngine;

public class SimpleDefenceCheck : MonoBehaviour
{
    public float defence;
    public void PassDefenceCheck(EntityHealth.Health health, float amount, bool clamp, Object sender)
    {
        if(amount < 0)
            health.UpdateHealth(amount / defence);
        else
            health.UpdateHealth(amount);
    }
}
