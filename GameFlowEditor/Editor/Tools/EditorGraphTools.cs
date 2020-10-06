namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Tools
{
    using System.Diagnostics;
    using System.Linq;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniGame.Core.Runtime.Extension;
    using UniGreenModules.UniGame.Core.EditorTools.Editor.DrawersTools;
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
            var graph = node?.GraphData as NodeGraph;
            if (node == null || !graph)
                return;

            if (node is Object assetNode) {
                assetNode.DrawOdinPropertyInspector();
            }
            else {
                DrawSerializableNode(node, graph);
            }
  
        }

        [Conditional("ODIN_INSPECTOR")]
        public static void DrawSerializableNode(this INode node, INodeGraph graph)
        {
            var graphAsset = graph as NodeGraph;
            if (!graphAsset) return;
            
            var isSerializable = node is SerializableNode;
            
            var collection = isSerializable ?
                graphAsset.serializableNodes: 
                graphAsset.ObjectNodes;

            var index = -1;
            for (int i = 0; i < collection.Count; i++) {
                var nodeItem = collection[i];
                if (nodeItem != node && nodeItem.Id != node.Id)
                    continue;
                index = i;
                break;
            }

            if (index < 0)
                return;
            
            var nodeType = isSerializable ? 
                graphAsset.serializableNodes.GetType() :
                graphAsset.nodes.GetType();
            
            var propertyTree = graphAsset.GetPropertyTree();
            
            for (var i = 0; i < propertyTree.RootPropertyCount; i++) {
                var p = propertyTree.GetRootProperty(i);
                if(p.Info.TypeOfValue != nodeType)
                    continue;
                var children = p.Children;
                if (children.Count <= index)
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
