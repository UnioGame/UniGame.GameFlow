namespace UniModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.GameFlowEditor.Editor.Tools
{
    using System.Linq;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniGame.Core.EditorTools.Editor.DrawersTools;
    using UniModules.UniGame.Core.Runtime.Extension;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class EditorGraphTools
    {
        public static UniGraph FindSceneGraph(string graphName)
        {
            var target = NodeGraph.ActiveGraphs.
                OfType<UniGraph>().
                FirstOrDefault(x => x.name == graphName);
            if (target) return target;
            target = Object.FindObjectsOfType<UniGraph>().
                FirstOrDefault(x => x.name == graphName);
            return target;
        }

        public static void DrawNodeImGui(this INode node)
        {
            var graph = node?.GraphData as UniGraph;
            if (node == null || !graph)
                return;

            if (node is Object assetNode) {
                assetNode.DrawOdinPropertyInspector();
                return;
            } 
  
        }

        public static void DrawSerializableNode(INode node, UniGraph graph)
        {          
            var isSerializable = node is SerializableNode;
            
            var index = isSerializable ?
                graph.serializableNodes.IndexOf(node) : 
                graph.ObjectNodes.IndexOf(node);
            
            if (index < 0)
                return;
            
            var nodeType = isSerializable ? 
                graph.serializableNodes.GetType() :
                graph.nodes.GetType();
            
            var propertyTree = graph.GetPropertyTree();
            
            for (var i = 0; i < propertyTree.RootPropertyCount; i++) {
                var p = propertyTree.GetRootProperty(i);
                if(p.Info.TypeOfValue != nodeType)
                    continue;
                var children = p.Children;
                if (children.Count <= index || index < 0)
                    return;
                
                var snode = children[index];
                var items = snode.Children;
                for (var j = 0; j < items.Count; j++) {
                    var property = items[j];
                    property.Draw();
                }
            }
            
        }

        public static VisualElement DrawNodeUiElements(this INode node)
        {
            var view = new IMGUIContainer(() => DrawNodeImGui(node));
            return view;
        }
    }
}
