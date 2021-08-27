namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
    using System.Diagnostics;
    using BaseEditor;
    using UniModules.GameFlow.Runtime.Core;
    using UniGame.GameFlowEditor.Editor;
    using UniModules.UniGame.Editor.DrawersTools;
    using UniModules.GameFlow.Editor;
    using UnityEditor;
    using UnityEngine;
    using Editor = UnityEditor.Editor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UniGraph))]
    public class UniNodesGraphEditor : Editor
    {
        private string _activeGraphName;
        private UniGraph _graph;

        public void Open(UniGraph graph)
        {
            if (graph == null)
                return;
            
            _graph = graph;
            _activeGraphName = _graph.name;
            
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

        public override void OnInspectorGUI()
        {
            var isOdinSupported = false;
#if ODIN_INSPECTOR
            isOdinSupported = true;
#endif
            if (isOdinSupported) {
                target.DrawOdinPropertyInspector();
            }
            else {
                base.OnInspectorGUI();
            }

            var graph = target as UniGraph;

            Open(graph);
        }

    }
}
