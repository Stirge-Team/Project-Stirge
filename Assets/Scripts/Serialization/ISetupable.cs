using UnityEngine;

namespace Stirge.Serialization
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
    public interface ISetupable<in TArg0, in TArg1, in TArg2, in TArg3>
    {
        public void Setup(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
    public interface ISetupable<in TArg0, in TArg1, in TArg2, in TArg3, in TArg4>
    {
        public void Setup(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }

    public interface INotSetupable { }
}
