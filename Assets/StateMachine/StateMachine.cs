using Sirenix.OdinInspector;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/Controller")]
    public sealed class StateMachine : SerializedScriptableObject
    {
#if UNITY_EDITOR
        public int statesCount = 0;
        public string[] names = new string[0];
        public Vector2[] positions = new Vector2[0];
#endif

        [SerializeField] private Action[][] actions = new Action[0][];
        [SerializeField] private int[][] transitions = new int[0][];
        [SerializeField] private Condition[][][] conditions = new Condition[0][][];

        [SerializeField] private int startStateIndex = -1;

        [System.NonSerialized] private int currentStateIndex;
        [System.NonSerialized] private bool changeState = false;

        public void OnStart()
        {
            currentStateIndex = startStateIndex;

            ExecuteActionsOnStart();
        }

        public void OnUpdate()
        {
            ExecuteActionsOnUpdate();

            CheckConditions();
        }

        public void OnEnd()
        {
            ExecuteActionsOnEnd();
        }

        private void ExecuteActionsOnStart()
        {
            for (int i = 0; i < actions[currentStateIndex].Length; i++)
                actions[currentStateIndex][i].OnStart();
        }

        private void ExecuteActionsOnUpdate()
        {
            for (int i = 0; i < actions[currentStateIndex].Length; i++)
                actions[currentStateIndex][i].OnUpdate();
        }

        private void ExecuteActionsOnEnd()
        {
            for (int i = 0; i < actions[currentStateIndex].Length; i++)
                actions[currentStateIndex][i].OnEnd();
        }

        private void CheckConditions()
        {
            for (int i = 0; i < conditions[currentStateIndex].Length; i++)
            {
                changeState = true;

                for (int j = 0; j < conditions[currentStateIndex][i].Length; j++)
                    if (!conditions[currentStateIndex][i][j].Check())
                    {
                        changeState = false;
                        break;
                    }

                if (changeState)
                {
                    ChangeState(transitions[currentStateIndex][i]);
                    return;
                }
            }
        }

        private void ChangeState(int newStateIndex)
        {
            Debug.Log(names[newStateIndex]);

            ExecuteActionsOnEnd();
            currentStateIndex = newStateIndex;
            ExecuteActionsOnStart();
        }
    }
}