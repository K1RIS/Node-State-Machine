using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace StateMachine
{
    public class StateMachineWindow : OdinEditorWindow
    {      
        private readonly Vector2 STATE_SIZE = new Vector2(150f, 50f);

        private GUIStyle startStateStyle;
        private GUIStyle normalStateStyle;

        private StateMachine stateMachine;
        private List<State> states = new List<State>();
        private int startStateIndex;

        private Vector2 scrollOffset = Vector2.zero;
        private int draggingStateIndex = -1;
        private int creatingTransitionFromStateIndex = -1;

        [OnOpenAsset(0)]
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

            window.states = new List<State>(StateMachineReflections.GetStates(stateMachine));
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
            for (int i = 0; i < states.Count; i++)
                GUI.Box(new Rect(states[i].position, STATE_SIZE), states[i].name, i == startStateIndex ? startStateStyle : normalStateStyle);
        }

        private void DrawTransitions()
        {
            foreach (State state in states)
                foreach (Transition transition in StateMachineReflections.GetTransitions(state))
                    DrawBezier(GetBezierStartPosition(state.position), GetBezierEndPosition(states[StateMachineReflections.GetTransitionStateIndex(transition)].position));
        }

        private void DrawCreatingTransition(Vector2 mousePosition)
        {
            if (creatingTransitionFromStateIndex == -1)
                return;

            DrawBezier(GetBezierStartPosition(states[creatingTransitionFromStateIndex].position), mousePosition);
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
                            EndCreatingTransition(e.mousePosition);
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
                        states = new List<State>(StateMachineReflections.GetStates(stateMachine));
                        startStateIndex = StateMachineReflections.GetStartStateIndex(stateMachine);
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
                genericMenu.AddItem(new GUIContent("Create Transition"), false, () => StartCreatingTransition(stateIndex));
                genericMenu.AddItem(new GUIContent("Set as Start State"), false, () => SetAsStartState(stateIndex));
                genericMenu.AddItem(new GUIContent("Delete State"), false, () => DeleteState(stateIndex));
            }
            else
            {
                Transition transition = GetTransition(mousePosition);

                if (transition != null)
                {
                    genericMenu.AddItem(new GUIContent("Delete Transition"), false, () => DeleteTransition(transition));
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
            if (indexState > -1 && indexState < states.Count)
                InspectObject(states[indexState]);
        }

        private void CreateState(Vector2 mousePosition)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "State Created");

            states.Add(new State("New State", mousePosition));
            StateMachineReflections.SetStates(stateMachine, states.ToArray());

            if (states.Count == 1)
                StateMachineReflections.SetStartStateIndex(stateMachine, startStateIndex = 0);
        }

        private void DeleteState(int stateIndex)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "State Deleted");

            RebuildTransitions(stateIndex);

            states.RemoveAt(stateIndex);
            StateMachineReflections.SetStates(stateMachine, states.ToArray());

            if (startStateIndex == stateIndex)
                StateMachineReflections.SetStartStateIndex(stateMachine, startStateIndex = states.Count > 0 ? 0 : -1);
        }

        private void RebuildTransitions(int stateToDeleteIndex)
        {
            List<Transition> transitions;

            foreach (State state in states)
            {
                transitions = new List<Transition>(StateMachineReflections.GetTransitions(state));

                for (int i = 0; i < transitions.Count; i++)
                {
                    int transitionStateIndex = StateMachineReflections.GetTransitionStateIndex(transitions[i]);

                    if (transitionStateIndex == stateToDeleteIndex)
                        transitions.RemoveAt(i--);
                    else if (transitionStateIndex > stateToDeleteIndex)
                        StateMachineReflections.SetTransitionStateIndex(stateMachine, transitions[i], transitionStateIndex - 1);
                }

                StateMachineReflections.SetTransitions(stateMachine, state, transitions.ToArray());
            }
        }

        private void SetAsStartState(int stateIndex)
        {
            StateMachineReflections.SetStartStateIndex(stateMachine, startStateIndex = stateIndex);
        }

        private void StartCreatingTransition(int stateIndex)
        {
            creatingTransitionFromStateIndex = stateIndex;
        }

        private void EndCreatingTransition(Vector2 mousePosition)
        {
            int stateIndex = GetStateIndex(mousePosition);

            if (stateIndex != -1 && stateIndex != creatingTransitionFromStateIndex && !StateMachineReflections.GetTransitions(states[creatingTransitionFromStateIndex]).Any(t => StateMachineReflections.GetTransitionStateIndex(t) == stateIndex))
            {
                Undo.RegisterCompleteObjectUndo(stateMachine, "Transition Created");

                List<Transition> newTransitions = new List<Transition>(StateMachineReflections.GetTransitions(states[creatingTransitionFromStateIndex]));
                newTransitions.Add(new Transition(stateIndex));
                StateMachineReflections.SetTransitions(stateMachine, states[creatingTransitionFromStateIndex], newTransitions.ToArray());
            }
        }

        private void DeleteTransition(Transition transitionToDelete)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "Transition Deleted");

            foreach (State state in states)
            {
                List<Transition> transitions = new List<Transition>(StateMachineReflections.GetTransitions(state));

                for (int i = 0; i < transitions.Count; i++)
                    if (transitions[i] == transitionToDelete)
                    {
                        transitions.RemoveAt(i);
                        StateMachineReflections.SetTransitions(stateMachine, state, transitions.ToArray());
                        return;
                    }

                StateMachineReflections.SetTransitions(stateMachine, state, transitions.ToArray());
            }
        }

        private void ResetPanning()
        {
            foreach (State state in states)
                state.position -= scrollOffset;

            scrollOffset = Vector2.zero;
        }

        private void Drag(Vector2 delta)
        {
            Undo.RegisterCompleteObjectUndo(stateMachine, "State Moved");

            states[draggingStateIndex].position += delta;

            GUI.changed = true;
        }

        private void Panning(Vector2 delta)
        {
            scrollOffset += delta;

            foreach (State state in states)
                state.position += delta;

            GUI.changed = true;
        }

        #region Helpers
        private int GetStateIndex(Vector2 mousePosition)
        {
            foreach (State state in states)
                if (new Rect(state.position, STATE_SIZE).Contains(mousePosition))
                {
                    int index = states.IndexOf(state);
                    if (index != -1)
                        return index;
                }

            return -1;
        }

        private Transition GetTransition(Vector2 mousePosition)
        {
            float minDist = 5f;
            Transition closestTransition = null;

            foreach (State state in states)
                foreach (Transition transition in StateMachineReflections.GetTransitions(state))
                {
                    float dist = GetDistanceToBezier(mousePosition, state.position, states[StateMachineReflections.GetTransitionStateIndex(transition)].position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestTransition = transition;
                    }
                }

            return closestTransition;
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