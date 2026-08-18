using UnityEngine;

namespace Stirge.UtilityAI
{
    public readonly struct StatusData
    {
        public StatusData(bool parameterless)
        {
            scaling = 1f;
            stackType = 0;
            durationType = 0;
            displayName = "New Status";
            maxStacks = 1;
            conditions = new Condition[0];
        }
        public StatusData(float _scaling, StatusStackType _stackType, StatusDurationType _durationType, string _displayName, int _maxStacks, Condition[] _conditions)
        {
            scaling = _scaling;
            stackType = _stackType;
            durationType = _durationType;
            displayName = _displayName;
            maxStacks = _maxStacks;
            conditions = _conditions;
        }
        
        public readonly float scaling;
        public readonly StatusStackType stackType;
        public readonly StatusDurationType durationType;
        public readonly string displayName;
        public readonly int maxStacks;
        public readonly Condition[] conditions;
    }
}
