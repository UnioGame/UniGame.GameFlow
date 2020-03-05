namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Nodes
{
    using System.Collections.Generic;
    using BaseEditor;
    using Drawers;
    using Drawers.Interfaces;
    using Runtime.Core;

    [CustomNodeEditor(typeof(UniGraphNode))]
    public class GraphNodeEditor : UniNodeEditor
    {
        protected override List<INodeEditorHandler> InitializeBodyHandlers(List<INodeEditorHandler> drawers)
        {
            base.InitializeBodyHandlers(drawers);
            drawers.Add(new ButtonActionBodyDrawer("show graph",ShowGraph));
            
            return drawers;
        }

        private void ShowGraph()
        {
            var graph = Node as UniGraphNode;
            if (graph == null) return;

            var targetGraph = graph.LoadOrigin();
            if (!targetGraph)
                return;
            
            UniGraphEditorWindow.Open(targetGraph);
        }
        


    }
}
