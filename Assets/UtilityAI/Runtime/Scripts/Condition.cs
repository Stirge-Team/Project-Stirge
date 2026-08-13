using Stirge.Combat;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    protected enum Operation
    {
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
    }

    [SerializeField] protected Operation m_operation;
    public abstract bool Evaluate(CombatEntity user, CombatEntity target);
}
