using System.Reflection;
using UnityEditor;

namespace StateMachine
{
    public static class StateMachineReflections
    {
        public static int[][] GetTransitions(StateMachineController stateMachine)
        {
            return (int[][])typeof(StateMachineController).GetField("transitions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetTransitions(StateMachineController stateMachine, int[][] transitionsIndexes)
        {
            typeof(StateMachineController).GetField("transitions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, transitionsIndexes);
            EditorUtility.SetDirty(stateMachine);
        }

        public static int GetStartStateIndex(StateMachineController stateMachine)
        {
            return (int)typeof(StateMachineController).GetField("startStateIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetStartStateIndex(StateMachineController stateMachine, int index)
        {
            typeof(StateMachineController).GetField("startStateIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, index);
            EditorUtility.SetDirty(stateMachine);
        }

        public static Action[][] GetActions(StateMachineController stateMachine)
        {
            return (Action[][])typeof(StateMachineController).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetActions(StateMachineController stateMachine, Action[][] actions)
        {
            typeof(StateMachineController).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, actions);
            EditorUtility.SetDirty(stateMachine);
        }

        public static Condition[][][] GetConditions(StateMachineController stateMachine)
        {
            return (Condition[][][])typeof(StateMachineController).GetField("conditions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetConditions(StateMachineController stateMachine, Condition[][][] conditions)
        {
            typeof(StateMachineController).GetField("conditions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, conditions);
            EditorUtility.SetDirty(stateMachine);
        }
    }
}