using Sirenix.OdinInspector;
using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public sealed class Transition
    {
        [SerializeField, OnValueChanged(nameof(SetConditionsCount))] private Condition[] conditions = new Condition[0];
        [SerializeField, HideInInspector] private int conditionsCount = 0;
        private void SetConditionsCount() => conditionsCount = conditions.Length;

        [SerializeField, HideInInspector] private int stateIndex = -1;

        public Transition(int stateIndex)
        {
            this.stateIndex = stateIndex;
        }

        public int CheckConditions()
        {
            for (int i = 0; i < conditionsCount; i++)
            {
                if (!conditions[i].Check())
                    return -1;
            }

            return stateIndex;
        }
    }
}