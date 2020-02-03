using UnityEngine;

namespace StateMachine
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool Check();
    }
}