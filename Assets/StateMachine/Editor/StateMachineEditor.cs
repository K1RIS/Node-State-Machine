using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [CustomEditor(typeof(StateMachine))]
    public class StateMachineEditor : Editor
    {
        private SerializedProperty currentState;

        private SerializedProperty stateName;

        private SerializedProperty startActions;
        private SerializedProperty updateActions;
        private SerializedProperty endActions;

        private string[] transitionNames;
        private SerializedProperty[] conditions;

        private void OnEnable()
        {
            StateMachineWindow.OpenWindow(target as StateMachine);
            StateMachineWindow.OnStateSelected += SetCurrentState;
        }

        private void OnDisable()
        {
            StateMachineWindow.OnStateSelected -= SetCurrentState;
        }

        public override void OnInspectorGUI()
        {
            if (currentState == null)
                return;

            EditorGUILayout.PropertyField(stateName);

            GUILayout.Space(20f);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(startActions, true);
            if (EditorGUI.EndChangeCheck())
                UpdateActionsCount("startActions", "startActionsCount");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(updateActions, true);
            if (EditorGUI.EndChangeCheck())
                UpdateActionsCount("updateActions", "updateActionsCount");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(endActions, true);
            if (EditorGUI.EndChangeCheck())
                UpdateActionsCount("endActions", "endActionsCount");

            GUILayout.Space(20f);

            for (int i = 0; i < conditions.Length; i++)
            {
                GUILayout.Label(transitionNames[i]);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(conditions[i], true);
                if (EditorGUI.EndChangeCheck())
                    UpdateConditionsCount(i);

                GUILayout.Space(10f);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetCurrentState(int index)
        {
            currentState = serializedObject.FindProperty("states").GetArrayElementAtIndex(index);

            stateName = currentState.FindPropertyRelative("name");

            startActions = currentState.FindPropertyRelative("startActions");
            updateActions = currentState.FindPropertyRelative("updateActions");
            endActions = currentState.FindPropertyRelative("endActions");


            SerializedProperty transitions = currentState.FindPropertyRelative("transitions");
            transitionNames = new string[transitions.arraySize];
            for (int i = 0; i < transitionNames.Length; i++)
                transitionNames[i] = "=> " + serializedObject.FindProperty("states").GetArrayElementAtIndex(transitions.GetArrayElementAtIndex(i).FindPropertyRelative("stateIndex").intValue).FindPropertyRelative("name").stringValue;

            conditions = new SerializedProperty[transitions.arraySize];
            for (int i = 0; i < conditions.Length; i++)
                conditions[i] = transitions.GetArrayElementAtIndex(i).FindPropertyRelative("conditions");

            Repaint();
        }

        private void UpdateActionsCount(string arrayName, string counterName)
        {
            currentState.FindPropertyRelative(counterName).intValue = currentState.FindPropertyRelative(arrayName).arraySize;
        }

        private void UpdateConditionsCount(int transitionIndex)
        {
            SerializedProperty transition = currentState.FindPropertyRelative("transitions").GetArrayElementAtIndex(transitionIndex);
            transition.FindPropertyRelative("conditionsCount").intValue = conditions[transitionIndex].arraySize;
        }
    }
}