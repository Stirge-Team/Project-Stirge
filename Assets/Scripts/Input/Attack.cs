using Stirge.Input;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private AttackInput m_followUpInput = 0;

    [SerializeField] private Attack m_followUpAttack = null;
    public void Use() { }
}