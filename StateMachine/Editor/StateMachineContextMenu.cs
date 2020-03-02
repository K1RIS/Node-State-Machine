using UnityEditor;
using UnityEngine;

namespace StateMachine.Editor
{
    public static class StateMachineContextMenu
    {
        [MenuItem("Assets/Create/State Machine/Controller", priority = 1)]
        private static void NewController()
        {
            ScriptableObject controller = ScriptableObject.CreateInstance(typeof(StateMachineController));
            AssetDatabase.CreateAsset(controller, AssetDatabase.GenerateUniqueAssetPath($"{AssetDatabase.GetAssetPath(Selection.activeObject)}/New State Machine Controller.asset"));

            ScriptableObject nodesInfo = ScriptableObject.CreateInstance(typeof(NodesInfo));
            nodesInfo.name = "Nodes";
            AssetDatabase.AddObjectToAsset(nodesInfo, controller);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/State Machine/Action", priority = 1)]
        private static void NewAction()
        {
            TryCreateScriptFromTemplate("Action_Template", "NewAction");
        }

        [MenuItem("Assets/Create/State Machine/Condition", priority = 1)]
        private static void NewCondition()
        {
            TryCreateScriptFromTemplate("Condition_Template", "NewCondition");
        }

        [MenuItem("Assets/Create/State Machine/Duration", priority = 1)]
        private static void NewDuration()
        {
            TryCreateScriptFromTemplate("Duration_Template", "NewDuration");
        }

        private static void TryCreateScriptFromTemplate(string templateName, string defaultScriptName)
        {
            string[] guids = AssetDatabase.FindAssets(templateName);
            if (guids.Length == 0)
                Debug.LogWarning(templateName + ".txt not found in asset database");
            else
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(AssetDatabase.GUIDToAssetPath(guids[0]), defaultScriptName + ".cs");
        }
    }
}