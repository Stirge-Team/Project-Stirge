using Stirge.Input;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private string m_name = "";
    [SerializeField] private AttackInput m_followUpInput = 0;
    [SerializeField] private Attack m_followUpAttack = null;

    public string Name => m_name;

    public void Use() { }
}