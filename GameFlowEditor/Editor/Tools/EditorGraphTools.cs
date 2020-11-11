namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Tools
{
    using System.Linq;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniModules.UniGame.Core.EditorTools.Editor.DrawersTools;
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

            graphAsset.DrawAssetChildWithOdin(nodeType, index);

        }

        public static VisualElement DrawNodeUiElements(this INode node)
        {
            var view = new IMGUIContainer(() => DrawNodeImGui(node));
            return view;
        }

        private static void DrawWithOdin()
        {
            
        }
    }
}
