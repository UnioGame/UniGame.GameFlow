using UnityEditor;

namespace UniGame.GameFlowEditor.Editor.DemoGraph
{
    using UnityEngine;
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(DemoGraphFlow))]
    public class UniGameFlowExampleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var targetGraph = target as DemoGraphFlow;
            var graph = targetGraph.graph;

            if (!graph) return;

            if (GUILayout.Button("SHOW")) {
                UniGameFlowWindow.Open(graph);
            }
            
            if (GUILayout.Button("SHOW NODES")) {
                DemoNodeViewWindow.Open(graph);
            }
        }
    }
}
