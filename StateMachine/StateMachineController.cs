using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public sealed class StateMachineController : SerializedScriptableObject
    {
        public string[] boolNames = new string[0];
        public bool[] boolValues = new bool[0];
        public string[] floatNames = new string[0];
        public float[] floatValues = new float[0];
        public string[] intNames = new string[0];
        public int[] intValues = new int[0];

        [SerializeField] private Action[][] actions = new Action[0][];
        [SerializeField] private int[][] transitions = new int[0][];
        [SerializeField] private Duration[] durations = new Duration[0];
        [SerializeField] private bool[][] isDurationsEnd = new bool[0][];
        [SerializeField] private Condition[][][] conditions = new Condition[0][][];

        [SerializeField] private int startStateIndex = -1;

        [System.NonSerialized] private int currentStateIndex;
        [System.NonSerialized] private int actionsCount;
        [System.NonSerialized] private int transitionsCount;
        [System.NonSerialized] private float duration;

        [System.NonSerialized] private int conditionsCount;
        [System.NonSerialized] private bool changeState;

        [System.NonSerialized] private float time;

        public void OnStart()
        {
            InitState(startStateIndex);

            ExecuteActionsOnStart();
        }

        private void InitState(int stateIndex)
        {
            currentStateIndex = stateIndex;
            actionsCount = actions[stateIndex].Length;
            transitionsCount = conditions[stateIndex].Length;
            duration = durations[stateIndex].Get();
        }

        public void OnUpdate(float delta)
        {
            time += delta;

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
                actions[currentStateIndex][i].OnUpdate(time, duration);
        }

        private void ExecuteActionsOnEnd()
        {
            for (int i = 0; i < actionsCount; i++)
                actions[currentStateIndex][i].OnEnd();
        }

        private void CheckConditions()
        {
            for (int transitionIndex = 0; transitionIndex < transitionsCount; transitionIndex++)
            {
                if (!isDurationsEnd[currentStateIndex][transitionIndex] || (isDurationsEnd[currentStateIndex][transitionIndex] && time > duration))
                {
                    conditionsCount = conditions[currentStateIndex][transitionIndex].Length;
                    changeState = true;

                    for (int conditionIndex = 0; conditionIndex < conditionsCount; conditionIndex++)
                        if (!conditions[currentStateIndex][transitionIndex][conditionIndex].Check())
                        {
                            changeState = false;
                            break;
                        }

                    if (changeState)
                    {
                        ChangeState(transitions[currentStateIndex][transitionIndex]);
                        return;
                    }
                }
            }
        }

        private void ChangeState(int stateIndex)
        {
            ExecuteActionsOnEnd();

            InitState(stateIndex);

            time = 0f;

            ExecuteActionsOnStart();
        }
    }
}