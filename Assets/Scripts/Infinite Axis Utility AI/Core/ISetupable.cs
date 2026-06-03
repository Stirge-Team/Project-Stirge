using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface ISetupable<in TArg>
    {
        public void Setup(TArg arg);
    }
    public interface ISetupable<in TArg0, in TArg1>
    {
        public void Setup(TArg0 arg0, TArg1 arg1);
    }
    public interface ISetupable<in TArg0, in TArg1, in TArg2>
    {
        public void Setup(TArg0 arg0, TArg1 arg1, TArg2 arg2);
    }

    public interface INotSetupable { }
}
