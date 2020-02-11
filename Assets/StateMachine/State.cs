using Sirenix.OdinInspector;
using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public sealed class State
    {
#if UNITY_EDITOR
        [SerializeField] public string name;
        [SerializeField, HideInInspector] public Vector2 position;

        private int SetActionsCount() => actionsCount = actions.Length;
#endif

        [SerializeField, OnValueChanged(nameof(SetActionsCount))] private Action[] actions = new Action[0];
        [SerializeField, HideInInspector] private int actionsCount = 0;

        [SerializeField] private Transition[] transitions = new Transition[0];
        [SerializeField, HideInInspector] private int transitionsCount = 0;

        private int transitonStateIndex;

        public State(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
        }

        public void OnStart()
        {
            for (int i = 0; i < actionsCount; i++)
                actions[i].OnStart();
        }
        public void OnUpdate()
        {
            for (int i = 0; i < actionsCount; i++)
                actions[i].OnUpdate();
        }
        public void OnEnd()
        {
            for (int i = 0; i < actionsCount; i++)
                actions[i].OnEnd();
        }

        public int ChackTransitions()
        {
            for (int i = 0; i < transitionsCount; i++)
            {
                transitonStateIndex = transitions[i].CheckConditions();

                if (transitonStateIndex != -1)
                    return transitonStateIndex;
            }

            return -1;
        }
    }
}