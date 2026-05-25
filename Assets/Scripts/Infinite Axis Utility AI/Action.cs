using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class Action
    {
        protected virtual void OnInitialise() { }

        protected virtual void OnBegin() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnEnd() { }

        protected virtual void OnDispose() { }
        
        public void Initialise()
        {
            OnInitialise();
        }

        public void Begin()
        {
            OnBegin();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void End()
        {
            OnEnd();
        }

        public void Dispose()
        {
            OnDispose();
        }
    }
}
