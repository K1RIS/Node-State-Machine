using Sirenix.OdinInspector;
using UnityEngine;

namespace StateMachine
{
    public sealed class StateMachineController : SerializedScriptableObject
    {
        [SerializeField] private Action[][] actions = new Action[0][];
        [SerializeField] private int[][] transitions = new int[0][];
        [SerializeField] private Condition[][][] conditions = new Condition[0][][];

        [SerializeField] private int startStateIndex = -1;

        [System.NonSerialized] private int currentStateIndex;
        [System.NonSerialized] private int actionsCount;
        [System.NonSerialized] private int transitionsCount;

        [System.NonSerialized] private int conditionsCount;
        [System.NonSerialized] private bool changeState;

        public void OnStart()
        {
            currentStateIndex = startStateIndex;
            actionsCount = actions[currentStateIndex].Length;
            transitionsCount = conditions[currentStateIndex].Length;
     
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
            for (int i = 0; i < actionsCount; i++)
                actions[currentStateIndex][i].OnStart();
        }

        private void ExecuteActionsOnUpdate()
        {
            for (int i = 0; i < actionsCount; i++)
                actions[currentStateIndex][i].OnUpdate();
        }

        private void ExecuteActionsOnEnd()
        {
            for (int i = 0; i < actionsCount; i++)
                actions[currentStateIndex][i].OnEnd();
        }

        private void CheckConditions()
        {
            for (int i = 0; i < transitionsCount; i++)
            {
                conditionsCount = conditions[currentStateIndex][i].Length;
                changeState = true;

                for (int j = 0; j < conditionsCount; j++)
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
            ExecuteActionsOnEnd();

            currentStateIndex = newStateIndex;
            actionsCount = actions[currentStateIndex].Length;
            transitionsCount = conditions[currentStateIndex].Length;

            ExecuteActionsOnStart();
        }
    }
}