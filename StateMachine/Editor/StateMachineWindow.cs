using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class StateMachineWindow : OdinEditorWindow
    {
        private class State
        {
            [Serializable]
            private class Transition
            {
                private string transitionTo;
                [ShowInInspector, ValueDropdown(nameof(GetUniqueConditions))] private Condition[] conditions;

                public Transition(string transitionTo, Condition[] conditions)
                {
                    this.transitionTo = transitionTo;
                    this.conditions = conditions;
                }

                private IEnumerable<Condition> GetUniqueConditions()
                {
                    return typeof(Condition).Assembly.GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(Condition)))
                        .Where(t => !conditions.Any(i => i.GetType() == t))
                        .Select(x => (Condition)Activator.CreateInstance(x));
                }

                public Condition[] GetConditions() => conditions;
            }

            private StateMachine stateMachine;
            private int stateIndex;

            [ShowInInspector, OnValueChanged(nameof(SaveName))] private string name;
            [ShowInInspector, OnValueChanged(nameof(SaveActions)), ValueDropdown(nameof(GetUniqueActions))] private Action[] actions;
            [ShowInInspector, OnValueChanged(nameof(SaveConditions)), ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "transitionTo")]
            private Transition[] transitions;


            public State(StateMachine stateMachine, int stateIndex)
            {
                this.stateMachine = stateMachine;
                this.stateIndex = stateIndex;

                name = stateMachine.names[stateIndex];

                actions = StateMachineReflections.GetActions(stateMachine)[stateIndex];

                int[] transitionsIndexes = StateMachineReflections.GetTransitions(stateMachine)[stateIndex];
                Condition[][] transitionConditions = StateMachineReflections.GetConditions(stateMachine)[stateIndex];
                transitions = new Transition[transitionsIndexes.Length];
                for (int i = 0; i < transitionsIndexes.Length; i++)
                    transitions[i] = new Transition(stateMachine.names[transitionsIndexes[i]], transitionConditions[i]);
            }

            private void SaveName() => stateMachine.names[stateIndex] = name;
            private IEnumerable<Action> GetUniqueActions()
            {
                return typeof(Action).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Action)))
                    .Where(t => !actions.Any(i => i.GetType() == t))
                    .Select(x => (Action)Activator.CreateInstance(x));
            }
            private void SaveActions()
            {
                Action[][] actions = StateMachineReflections.GetActions(stateMachine);
                actions[stateIndex] = this.actions;
                StateMachineReflections.SetActions(stateMachine, actions);
            }

            [Button]
            private void SaveConditions()
            {
                Condition[][][] conditions = StateMachineReflections.GetConditions(stateMachine);
                conditions[stateIndex] = transitions.Select(i => i.GetConditions()).ToArray();
                StateMachineReflections.SetConditions(stateMachine, conditions);
            }
        }

        private readonly Vector2 STATE_SIZE = new Vector2(150f, 50f);

        private GUIStyle startStateStyle;
        private GUIStyle normalStateStyle;

        private StateMachine stateMachine;
        private int statesCount;
        private List<string> names = new List<string>();
        private List<Vector2> positions = new List<Vector2>();
        [SerializeField, HideInInspector] private List<List<int>> transitions = new List<List<int>>();
        private int startStateIndex;

        private Vector2 scrollOffset = Vector2.zero;
        private int draggingStateIndex = -1;
        private int creatingTransitionFromStateIndex = -1;

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            StateMachine statemachine = EditorUtility.InstanceIDToObject(instanceID) as StateMachine;
            if (statemachine != null)
            {
                OpenWindow(statemachine);
                return true;
            }
            return false;
        }

        [MenuItem("Window/State Machine")]
        private static void OpenWindow()
        {
            StateMachineWindow window = GetWindow<StateMachineWindow>();
            window.titleContent = new GUIContent("State Machine");
            window.minSize = new Vector2(800, 600);
        }

        public static void OpenWindow(StateMachine stateMachine)
        {
            StateMachineWindow window = GetWindow<StateMachineWindow>();
            window.titleContent = new GUIContent("State Machine");
            window.minSize = new Vector2(800, 600);
            window.stateMachine = stateMachine;

            window.statesCount = stateMachine.statesCount;
            window.names = new List<string>(stateMachine.names);
            window.positions = new List<Vector2>(stateMachine.positions);
            window.transitions = new List<List<int>>(StateMachineReflections.GetTransitions(stateMachine).Select(i => i.ToList()));
            window.startStateIndex = StateMachineReflections.GetStartStateIndex(stateMachine);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            startStateStyle = new GUIStyle();
            startStateStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png") as Texture2D;
            startStateStyle.border = new RectOffset(12, 12, 12, 12);
            startStateStyle.alignment = TextAnchor.MiddleCenter;

            normalStateStyle = new GUIStyle();
            normalStateStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            normalStateStyle.border = new RectOffset(12, 12, 12, 12);
            normalStateStyle.alignment = TextAnchor.MiddleCenter;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            DrawNodes();
            DrawTransitions();
            DrawCreatingTransition(Event.current.mousePosition);

            ProcessEvents(Event.current);

            if (GUI.changed)
                Repaint();
        }

        #region Drawers
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            Vector3 newOffset = new Vector3(scrollOffset.x % gridSpacing, scrollOffset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);

            for (int i = 0; i < heightDivs; i++)
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset, new Vector3(position.width, gridSpacing * i, 0f) + newOffset);

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNodes()
        {
            for (int stateIndex = 0; stateIndex < statesCount; stateIndex++)
                GUI.Box(new Rect(positions[stateIndex], STATE_SIZE), names[stateIndex], stateIndex == startStateIndex ? startStateStyle : normalStateStyle);
        }

        private void DrawTransitions()
        {
            for (int stateIndex = 0; stateIndex < statesCount; stateIndex++)
                for (int transitionIndex = 0; transitionIndex < transitions[stateIndex].Count; transitionIndex++)
                    DrawBezier(GetBezierStartPosition(positions[stateIndex]), GetBezierEndPosition(positions[transitions[stateIndex][transitionIndex]]));
        }

        private void DrawCreatingTransition(Vector2 mousePosition)
        {
            if (creatingTransitionFromStateIndex == -1)
                return;

            DrawBezier(GetBezierStartPosition(positions[creatingTransitionFromStateIndex]), mousePosition);
            GUI.changed = true;
        }
        #endregion

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (creatingTransitionFromStateIndex != -1)
                            CreateTransition(e.mousePosition);
                        else
                        {
                            SelectState(GetStateIndex(e.mousePosition));
                            draggingStateIndex = GetStateIndex(e.mousePosition);
                        }
                    }
                    else if (e.button == 1)
                        ProcessContextMenu(e.mousePosition);

                    creatingTransitionFromStateIndex = -1;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && draggingStateIndex != -1)
                        Drag(e.delta);
                    else if (e.button == 2)
                        Panning(e.delta);
                    break;

                case EventType.MouseUp:
                    if (e.button == 0)
                        draggingStateIndex = -1;
                    break;

                case EventType.ValidateCommand:
                    if (e.commandName == "UndoRedoPerformed")
                    {
                        //states = new List<State>(StateMachineReflections.GetStates(stateMachine));
                        //startStateIndex = StateMachineReflections.GetStartStateIndex(stateMachine);
                        GUI.changed = true;
                    }
                    break;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();

            int stateIndex = GetStateIndex(mousePosition);

            if (stateIndex != -1)
            {
                genericMenu.AddItem(new GUIContent("Create Transition"), false, () => creatingTransitionFromStateIndex = stateIndex);
                genericMenu.AddItem(new GUIContent("Set as Start State"), false, () => StateMachineReflections.SetStartStateIndex(stateMachine, startStateIndex = stateIndex));
                genericMenu.AddItem(new GUIContent("Delete State"), false, () => DeleteState(stateIndex));
            }
            else
            {
                Vector2Int transition = GetTransition(mousePosition);

                if (transition != new Vector2Int(-1, -1))
                {
                    genericMenu.AddItem(new GUIContent("Delete Transition"), false, () => DeleteTransition(transition.x, transition.y));
                }
                else
                {
                    genericMenu.AddItem(new GUIContent("New State"), false, () => CreateState(mousePosition));
                    genericMenu.AddItem(new GUIContent("Reset Panning"), false, ResetPanning);
                }
            }

            genericMenu.ShowAsContext();
        }

        private void SelectState(int indexState)
        {
            if (indexState != -1)
                InspectObjectInDropDown(new State(stateMachine, indexState));
        }

        private void CreateState(Vector2 mousePosition)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "State Created");

            statesCount++;
            names.Add("New State");
            positions.Add(mousePosition);
            transitions.Add(new List<int>());

            List<Action[]> actions = new List<Action[]>(StateMachineReflections.GetActions(stateMachine));
            actions.Add(new Action[0]);
            List<Condition[][]> conditions = new List<Condition[][]>(StateMachineReflections.GetConditions(stateMachine));
            conditions.Add(new Condition[0][]);

            SaveStateMachine(actions.ToArray(), conditions.ToArray());

            if (statesCount == 1)
                StateMachineReflections.SetStartStateIndex(stateMachine, startStateIndex = 0);
        }

        private void DeleteState(int stateIndex)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "State Deleted");

            DeleteStateReferencesInTransitions(stateIndex);

            statesCount--;
            names.RemoveAt(stateIndex);
            positions.RemoveAt(stateIndex);
            transitions.RemoveAt(stateIndex);

            List<Action[]> actions = new List<Action[]>(StateMachineReflections.GetActions(stateMachine));
            actions.RemoveAt(stateIndex);
            List<Condition[][]> conditions = new List<Condition[][]>(StateMachineReflections.GetConditions(stateMachine));
            conditions.RemoveAt(stateIndex);

            SaveStateMachine(actions.ToArray(), conditions.ToArray());

            if (startStateIndex == stateIndex)
                StateMachineReflections.SetStartStateIndex(stateMachine, startStateIndex = statesCount > 0 ? 0 : -1);
        }

        private void DeleteStateReferencesInTransitions(int stateIndexToDelete)
        {
            Condition[][][] conditions = StateMachineReflections.GetConditions(stateMachine);

            for (int stateIndex = 0; stateIndex < statesCount; stateIndex++)
                for (int transitionIndex = 0; transitionIndex < transitions[stateIndex].Count; transitionIndex++)
                    if (transitions[stateIndex][transitionIndex] == stateIndexToDelete)
                    {
                        transitions[stateIndex].RemoveAt(transitionIndex);

                        List<Condition[]> stateConditions = new List<Condition[]>(conditions[transitionIndex]);
                        stateConditions.RemoveAt(transitionIndex);
                        conditions[transitionIndex] = stateConditions.ToArray();

                        transitionIndex--;
                    }

            StateMachineReflections.SetConditions(stateMachine, conditions);
        }

        private void CreateTransition(Vector2 mousePosition)
        {
            int stateIndex = GetStateIndex(mousePosition);

            if (stateIndex != -1 && stateIndex != creatingTransitionFromStateIndex && !transitions[creatingTransitionFromStateIndex].Any(t => t == stateIndex))
            {
                Undo.RegisterCompleteObjectUndo(stateMachine, "Transition Created");

                transitions[creatingTransitionFromStateIndex].Add(stateIndex);
                StateMachineReflections.SetTransitions(stateMachine, transitions.Select(a => a.ToArray()).ToArray());

                Condition[][][] conditions = StateMachineReflections.GetConditions(stateMachine);
                List<Condition[]> stateConditions = new List<Condition[]>(conditions[creatingTransitionFromStateIndex]);
                stateConditions.Add(new Condition[0]);
                conditions[creatingTransitionFromStateIndex] = stateConditions.ToArray();
                StateMachineReflections.SetConditions(stateMachine, conditions);
            }
        }

        private void DeleteTransition(int stateIndex, int transitionIndex)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "Transition Deleted");

            transitions[stateIndex].RemoveAt(transitionIndex);
            StateMachineReflections.SetTransitions(stateMachine, transitions.Select(a => a.ToArray()).ToArray());

            Condition[][][] conditions = StateMachineReflections.GetConditions(stateMachine);
            List<Condition[]> stateConditions = new List<Condition[]>(conditions[stateIndex]);
            stateConditions.RemoveAt(transitionIndex);
            conditions[stateIndex] = stateConditions.ToArray();
            StateMachineReflections.SetConditions(stateMachine, conditions);
        }

        private void ResetPanning()
        {
            for (int i = 0; i < statesCount; i++)
                positions[i] -= scrollOffset;

            scrollOffset = Vector2.zero;
        }

        private void Drag(Vector2 delta)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "State Moved");

            positions[draggingStateIndex] += delta;

            GUI.changed = true;
        }

        private void Panning(Vector2 delta)
        {
            scrollOffset += delta;

            for (int i = 0; i < statesCount; i++)
                positions[i] += delta;

            GUI.changed = true;
        }

        #region Helpers
        private void SaveStateMachine(Action[][] actions, Condition[][][] conditions)
        {
            stateMachine.statesCount = statesCount;
            stateMachine.names = names.ToArray();
            stateMachine.positions = positions.ToArray();
            StateMachineReflections.SetTransitions(stateMachine, transitions.Select(a => a.ToArray()).ToArray());
            StateMachineReflections.SetActions(stateMachine, actions);
            StateMachineReflections.SetConditions(stateMachine, conditions);
        }

        private int GetStateIndex(Vector2 mousePosition)
        {
            for (int stateIndex = 0; stateIndex < statesCount; stateIndex++)
                if (new Rect(positions[stateIndex], STATE_SIZE).Contains(mousePosition))
                    return stateIndex;

            return -1;
        }

        private Vector2Int GetTransition(Vector2 mousePosition)
        {
            float minDist = 5f;
            Vector2Int stateAndTransitionIndex = new Vector2Int(-1, -1);

            for (int stateIndex = 0; stateIndex < statesCount; stateIndex++)
                for (int transitionIndex = 0; transitionIndex < transitions[stateIndex].Count; transitionIndex++)
                {
                    float dist = GetDistanceToBezier(mousePosition, positions[stateIndex], positions[transitions[stateIndex][transitionIndex]]);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        stateAndTransitionIndex.x = stateIndex;
                        stateAndTransitionIndex.y = transitionIndex;
                    }
                }

            return stateAndTransitionIndex;
        }

        private float GetDistanceToBezier(Vector2 mousePosition, Vector2 startPos, Vector2 endPos)
        {
            return HandleUtility.DistancePointBezier
                   (
                   mousePosition,
                   GetBezierStartPosition(startPos),
                   GetBezierEndPosition(endPos),
                   GetBezierStartPosition(startPos) + Vector2.right * 50f,
                   GetBezierEndPosition(endPos) + Vector2.left * 50f
                   );
        }

        private Vector2 GetBezierStartPosition(Vector2 statePos) => statePos + Vector2.right * STATE_SIZE.x + Vector2.up * STATE_SIZE.y * .5f;
        private Vector2 GetBezierEndPosition(Vector2 endPos) => endPos + Vector2.up * STATE_SIZE.y * .5f;

        private void DrawBezier(Vector2 startPos, Vector2 endPos)
        {
            Handles.DrawBezier
            (
                startPos,
                endPos,
                startPos + Vector2.right * 50f,
                endPos + Vector2.left * 50f,
                Color.white,
                null,
                10f
            );
        }
        #endregion
    }
}