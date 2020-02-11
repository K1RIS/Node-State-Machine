using Sirenix.OdinInspector;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/Controller")]
    public sealed class StateMachine : SerializedScriptableObject
    {
        private State[] states = new State[0];
        private int startStateIndex = -1;

        [System.NonSerialized] private int currentStateIndex;
        [System.NonSerialized] private int changeStateIndex;

        public void OnStart()
        { 
            currentStateIndex = startStateIndex;

            states[currentStateIndex].OnStart();
        }

        public void OnUpdate()
        {
            states[currentStateIndex].OnUpdate();

            changeStateIndex = states[currentStateIndex].ChackTransitions();
            if (changeStateIndex != -1)
                ChangeState();
        }

        public void OnEnd()
        {
            states[currentStateIndex].OnEnd();
        }

        private void ChangeState()
        {
            Debug.Log(states[changeStateIndex].name);

            states[currentStateIndex].OnEnd();
            currentStateIndex = changeStateIndex;
            states[currentStateIndex].OnStart();
        }
    }
}