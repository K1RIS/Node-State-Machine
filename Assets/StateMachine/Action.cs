using UnityEngine;

namespace StateMachine
{
    public abstract class Action : ScriptableObject
    {
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnEnd() { }
    }
}