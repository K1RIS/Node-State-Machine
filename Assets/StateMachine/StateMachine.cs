using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/Controller")]
    public class StateMachine : ScriptableObject
    {
        [SerializeField] private State[] states = new State[0];
        [SerializeField] private int startStateIndex = -1;

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
            states[currentStateIndex].OnEnd();
            currentStateIndex = changeStateIndex;
            states[currentStateIndex].OnStart();
        }
    }
}