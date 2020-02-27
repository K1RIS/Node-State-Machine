using UnityEditor;
using UnityEngine;

namespace StateMachine.Editor
{
    public static class StateMachineControllerCreator
    {
        [MenuItem("Assets/Create/State Machine Controller", priority = 1)]
        public static void Create()
        {
            ScriptableObject controller = ScriptableObject.CreateInstance(typeof(StateMachineController));
            AssetDatabase.CreateAsset(controller, AssetDatabase.GenerateUniqueAssetPath($"{AssetDatabase.GetAssetPath(Selection.activeObject)}/New State Machine Controller.asset"));

            ScriptableObject nodesInfo = ScriptableObject.CreateInstance(typeof(NodesInfo));
            nodesInfo.name = "Nodes";
            AssetDatabase.AddObjectToAsset(nodesInfo, controller);
            AssetDatabase.SaveAssets();
        }
    }
}