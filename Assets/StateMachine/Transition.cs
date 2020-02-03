using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public class Transition
    {
        [SerializeField] private Condition[] conditions = new Condition[0];
        [SerializeField] private int conditionsCount = 0;

        [SerializeField] private int stateIndex = -1;

        public Transition(int stateIndex)
        {
            this.stateIndex = stateIndex;
        }

        public int CheckConditions()
        {
            for (int i = 0; i < conditionsCount; i++)
                if (!conditions[i].Check())
                    return -1;

            return stateIndex;
        }
    }
}