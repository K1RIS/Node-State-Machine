using UnityEngine;

namespace StateMachine
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute();
    }
}