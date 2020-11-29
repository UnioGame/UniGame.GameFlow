using GraphProcessor;

namespace UniGame.GameFlowEditor.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.Attributes;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEngine;
    using Vector2 = UnityEngine.Vector2;

    public class UniAssetGraph : BaseGraph
    {
        #region static data

        private static MemorizeItem<Type, Type> nodeDataMap =
            MemorizeTool.Memorize<Type, Type>(nodeType =>
            {
                var baseType      = typeof(UniBaseNode);
                var allDataNodes  = baseType.GetAssignableWithAttributeMap<NodeBindAttribute>();
                var attributePair = allDataNodes.
                    Where(x => x.attribute!=null).
                    FirstOrDefault(x => x.attribute.NodeType == nodeType);
                var attribute     = attributePair.attribute;
                return attribute == null ? typeof(UniBaseNode) : attribute.NodeData;
            });

        #endregion
            
        private        UniGraph sourceGraph;

        public Dictionary<int,UniBaseNode> uniNodes = new Dictionary<int,UniBaseNode>(16);

        public UniGraph UniGraph => sourceGraph;

        public void Activate(UniGraph graph)
        {
            sourceGraph = graph;
            //update dynamic graph ports
            UpdateGraph();
        }

        public void RemoveUniNode(BaseNode node)
        {
            if (node is UniBaseNode targetNode) {
                sourceGraph.RemoveNode(targetNode.SourceNode);
            }
            RemoveNode(node);
        }

        public UniBaseNode CreateNode(Type type, Vector2 nodePosition)
        {
            var name = type.Name;
            
            #if UNITY_EDITOR
            name = UnityEditor.ObjectNames.NicifyVariableName(name);
            #endif
            var newNode = sourceGraph.AddNode(
                type,
                name,
                nodePosition);
            
            return CreateNode(newNode);
        }

        public UniBaseNode CreateNode(INode node)
        {
            var nodeType  = node.GetType();
            var dataType  = nodeDataMap.GetValue(nodeType);
            var graphNode = BaseNode.CreateFromType(dataType,node.Position) as UniBaseNode;
            if (graphNode == null)
            {
                Debug.LogError($"NULL Node bind with UniNode : {node}");
                return null;
            }
            
            graphNode.Initialize(node);
            
            //register node into all nodes list
            AddNode(graphNode);
            
            //register only uni nodes
            uniNodes[node.Id] = graphNode;
            
            //sourceGraph.Save();
            return graphNode;
        }
        
        public void UpdateGraph()
        {
            CreateNodes();
            ConnectNodePorts();
        }

        private void CreateNodes()
        {
            foreach (var node in sourceGraph.Nodes) {
                CreateNode(node);
            }
        }

        private void ConnectNodePorts()
        {
            foreach (var nodeItem in uniNodes) {
                var nodeView = nodeItem.Value;
                var node     = nodeView.SourceNode;
                foreach (var outputPortView in nodeView.outputPorts) {
                    
                    var portData = outputPortView.portData;
                    var sourcePort = node.GetPort(portData.displayName);
                    
                    foreach (var connection in sourcePort.Connections) {
                        if(!uniNodes.TryGetValue(connection.NodeId,out var connectionNode))
                            continue;
                        var targetNode = connectionNode.SourceNode;
                        var port = targetNode.GetPort(connection.PortName);
                        
                        if(port.Direction != PortIO.Input)
                            continue;
                        
                        var inputPortView = connectionNode.
                            GetPort(nameof(connectionNode.inputs),connection.PortName);
                        
                        Connect(inputPortView,outputPortView);
                    }
                }
            }
        }
    }
}
