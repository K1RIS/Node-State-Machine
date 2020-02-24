using System.Reflection;
using UnityEditor;

namespace StateMachine
{
    public static class StateMachineReflections
    {
        public static int[][] GetTransitions(StateMachine stateMachine)
        {
            return (int[][])typeof(StateMachine).GetField("transitions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetTransitions(StateMachine stateMachine, int[][] transitionsIndexes)
        {
            typeof(StateMachine).GetField("transitions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, transitionsIndexes);
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

        public static Action[][] GetActions(StateMachine stateMachine)
        {
            return (Action[][])typeof(StateMachine).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetActions(StateMachine stateMachine, Action[][] actions)
        {
            typeof(StateMachine).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, actions);
            EditorUtility.SetDirty(stateMachine);
        }

        public static Condition[][][] GetConditions(StateMachine stateMachine)
        {
            return (Condition[][][])typeof(StateMachine).GetField("conditions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stateMachine);
        }
        public static void SetConditions(StateMachine stateMachine, Condition[][][] conditions)
        {
            typeof(StateMachine).GetField("conditions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(stateMachine, conditions);
            EditorUtility.SetDirty(stateMachine);
        }
    }
}