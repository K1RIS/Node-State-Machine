using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public sealed class State
    {
#if UNITY_EDITOR
        [SerializeField] public string name;
        [SerializeField] public Vector2 position;
#endif

        [SerializeField] private Action[] startActions = new Action[0];
        [SerializeField] private Action[] updateActions = new Action[0];
        [SerializeField] private Action[] endActions = new Action[0];
        [SerializeField, HideInInspector] private int startActionsCount = 0;
        [SerializeField, HideInInspector] private int updateActionsCount = 0;
        [SerializeField, HideInInspector] private int endActionsCount = 0;

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
            for (int i = 0; i < startActionsCount; i++)
                startActions[i].Execute();
        }
        public void OnUpdate()
        {
            for (int i = 0; i < updateActionsCount; i++)
                updateActions[i].Execute();
        }
        public void OnEnd()
        {
            for (int i = 0; i < endActionsCount; i++)
                endActions[i].Execute();
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