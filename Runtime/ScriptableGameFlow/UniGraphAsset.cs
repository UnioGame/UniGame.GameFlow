using GraphProcessor;
using UniGame.GameFlow;
using UniModules.UniCore.Runtime.ObjectPool.Runtime;
using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
using UniRx;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniGame.GameFlowEditor.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;
    using Vector2 = UnityEngine.Vector2;


    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/GameFlow/UniGraphAsset", fileName = "UniGraphAsset")]
    public class UniGraphAsset : BaseGraph
    {
        #region static data

        public static MemorizeItem<Type, Type> NodeDataMap = MemorizeTool
            .Memorize<Type, Type>(nodeType => {
                var attribute = nodeType.GetCustomAttribute<NodeAssetAttribute>();
                return attribute == null ? typeof(UniBaseNode) : attribute.NodeType;
            });

        #endregion

        public UniGraph sourceGraph;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        [SerializeReference]
        public List<IUniExposedParameter> uniExposedParameters = new List<IUniExposedParameter>();

        public Dictionary<int, UniBaseNode> uniNodes = new Dictionary<int, UniBaseNode>(16);

        public UniGraph UniGraph => sourceGraph;

        public void ConnectToGraph(UniGraph graph)
        {
            sourceGraph = graph;
            //update dynamic graph ports
            UpdateGraph();
        }

        public void RemoveUniNode(BaseNode node)
        {
            if (node is UniBaseNode targetNode)
                sourceGraph.RemoveNode(targetNode.SourceNode);
            RemoveNode(node);
        }

        public UniBaseNode CreateNode(Type type, Vector2 nodePosition)
        {
            var nodeName = type.Name;

#if UNITY_EDITOR
            nodeName = UnityEditor.ObjectNames.NicifyVariableName(nodeName);
#endif
            var newNode = sourceGraph.AddNode(type, nodeName, nodePosition);
            var node = CreateNode(newNode);
            
            MessageBroker.Default.Publish(new UniGraphSaveMessage() { graph = sourceGraph });
            
            return node;
        }
        
        public UniBaseNode CreateNode(INode node)
        {
            var graphNode = nodes.FirstOrDefault(x => x is UniBaseNode baseNode && baseNode.sourceId == node.Id) as UniBaseNode;
            graphNode ??= CreateUniBaseNode(node);
            graphNode.Initialize(node,sourceGraph);
            
            //register only uni nodes
            uniNodes[node.Id] = graphNode;
            
            return graphNode;
        }

        public UniBaseNode CreateUniBaseNode(INode node)
        {
            var nodeType  = node.GetType();
            var dataType  = NodeDataMap.GetValue(nodeType);
            var graphNode = BaseNode.CreateFromType(dataType, node.Position) as UniBaseNode;
            
            //register node into all nodes list
            AddNode(graphNode);
            
            if (graphNode != null) 
                return graphNode;
            
            Debug.LogError($"NULL Node bind with UniNode : {node}");
            return null;
        }

        public void UpdateGraph()
        {
            CreateNodes();
            ValidateGraph();
            ConnectNodePorts();
        }

        private void ValidateGraph()
        {
            var removed = ClassPool.Spawn<List<BaseNode>>();
            foreach (var node in nodes)
            {
                if (node is UniBaseNode uniBaseNode && !uniNodes.ContainsKey(uniBaseNode.sourceId))
                    removed.Add(node);
            }
            
            removed.ForEach(RemoveNode);
            removed.Despawn();

            uniExposedParameters.RemoveAll(x => x == null);
        }

        private void CreateNodes()
        {
            foreach (var node in sourceGraph.Nodes)
            {
                if(node is IRuntimeOnlyNode) 
                    continue;
                CreateNode(node);
            }
            
        }

        private void ConnectNodePorts()
        {
            DisconnectUniNodeEdges();
            CreatePortConnections();
        }

        private void CreatePortConnections()
        {
            var portsConnections = ClassPool.Spawn<List<NodePortConnection>>();
            
            foreach (var nodeItem in uniNodes)
            {
                var nodeView = nodeItem.Value;
                var node = nodeView.SourceNode;
                foreach (var outputPortView in nodeView.outputPorts)
                {
                    var portData = outputPortView.portData;
                    var sourcePort = node.GetPort(portData.displayName);

                    foreach (var connection in sourcePort.Connections)
                    {
                        if (!uniNodes.TryGetValue(connection.NodeId, out var connectionNode)) 
                            continue;
                        
                        var targetNode = connectionNode.SourceNode;
                        var port = targetNode.GetPort(connection.PortName);

                        if (port.Direction != PortIO.Input) continue;

                        var inputPortView = connectionNode.GetPort(nameof(connectionNode.inputs), connection.PortName);

                        portsConnections.Add(new NodePortConnection()
                        {
                            source = inputPortView,
                            target = outputPortView
                        });
                    }
                }
            }
            
            foreach (var nodePortConnection in portsConnections)
            {
                var inputPortView = nodePortConnection.source;
                var outputPortView = nodePortConnection.target;
                
                Connect(inputPortView, outputPortView);
            }

            portsConnections.Despawn();
        }

        private void DisconnectUniNodeEdges()
        {
            edges.RemoveAll(x => x.inputNode is UniBaseNode && x.outputNode is UniBaseNode);
        }

    }
    
    public struct NodePortConnection
    {
        public GraphProcessor.NodePort source;
        public GraphProcessor.NodePort target;
    }
}