namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
    using System;
    using System.Linq;
    using BaseEditor;
    using Runtime.Core;
    using UniGame.GameFlowEditor.Editor;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
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
            base.OnInspectorGUI();
        
            var graph = target as UniGraph;

            Open(graph);
        }
    }
}
