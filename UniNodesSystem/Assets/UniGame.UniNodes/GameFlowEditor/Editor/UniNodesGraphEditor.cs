namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
    using BaseEditor;
    using Runtime.Core;
    using UniGame.GameFlowEditor.Editor;
    using UnityEditor;
    using UnityEngine;
    using Editor = UnityEditor.Editor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UniGraph))]
    public class UniNodesGraphEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            var graph = target as UniGraph;
        
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
        
            if (GUILayout.Button("Show Graph(obsolete)", GUILayout.Height(26)))
            {
                NodeEditorWindow.Open(graph);
            }
            if (GUILayout.Button("Show Graph", GUILayout.Height(26)))
            {
                UniGameFlowWindow.Open(graph);
            }   

        
            GUILayout.EndVertical();
        
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
    }
}
