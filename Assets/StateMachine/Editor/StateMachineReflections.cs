using System.Reflection;
using UnityEditor;

namespace StateMachine
{
    public static class StateMachineReflections
    {
        public static State[] GetStates(StateMachine stateMachine)
        {
            return (State[])typeof(StateMachine).GetField("states", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetStates(StateMachine stateMachine, State[] states)
        {
            typeof(StateMachine).GetField("states", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, states);
            EditorUtility.SetDirty(stateMachine);
        }

        public static int GetStartStateIndex(StateMachine stateMachine)
        {
            return (int)typeof(StateMachine).GetField("startStateIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetStartStateIndex(StateMachine stateMachine, int index)
        {
            typeof(StateMachine).GetField("startStateIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, index);
            EditorUtility.SetDirty(stateMachine);
        }

        public static Transition[] GetTransitions(State state)
        {
            return (Transition[])typeof(State).GetField("transitions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(state);
        }
        public static void SetTransitions(StateMachine stateMachine, State state, Transition[] transitions)
        {
            typeof(State).GetField("transitions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(state, transitions);
            typeof(State).GetField("transitionsCount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(state, transitions.Length);
            EditorUtility.SetDirty(stateMachine);
        }

        public static int GetTransitionStateIndex(Transition transition)
        {
            return (int)typeof(Transition).GetField("stateIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(transition);
        }
        public static void SetTransitionStateIndex(StateMachine stateMachine, Transition transition, int index)
        {
            typeof(Transition).GetField("stateIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(transition, index);
            EditorUtility.SetDirty(stateMachine);
        }

        public static Action[] GetActions(State state)
        {
            return (Action[])typeof(State).GetField("startActions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(state);
        }
        public static void SetActions(StateMachine stateMachine, State state, Action[] actions)
        {
            typeof(State).GetField("startActions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(state, actions);
            EditorUtility.SetDirty(stateMachine);
        }

        public static Condition[] GetConditions(Transition transition)
        {
            return (Condition[])typeof(Transition).GetField("conditions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(transition);
        }
    }
}