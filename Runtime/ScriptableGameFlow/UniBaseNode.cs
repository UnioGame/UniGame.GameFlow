using GraphProcessor;
using UniModules.UniGame.Core.Runtime.Rx;
using UniRx;

namespace UniGame.GameFlowEditor.Runtime
{
    using System;
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;
    using PortData = PortData;

    [HideNode]
    [Serializable]
    public class UniBaseNode : BaseNode
    {
        #region inspector

        public int sourceId;
        public string nodeName;
        public UniGraph sourceGraph;
        
        #endregion
        
        [Input(name = nameof(inputs), allowMultiple = true)]
        public IEnumerable< object > inputs = null;
        
        [Output(name = nameof(outputs), allowMultiple = true)]
        public IEnumerable< object > outputs = null;
        
        public Dictionary<INodePort,PortData> portData = new Dictionary<INodePort, PortData>(8);

        private ReactiveValue<INode> nodeSubject = new ReactiveValue<INode>();

        #region public properties

        public INode SourceNode => sourceGraph?.GetNode(sourceId);

        public IObservable<INode> SourceNodeObservable => nodeSubject;

        public override string name => SourceNode == null ? base.name : SourceNode.ItemName;

        public override string layoutStyle => GetNodeStyle();
        
        #endregion

        public UniBaseNode()
        {
            nodeSubject ??= new ReactiveValue<INode>();
        }
        
        public void Initialize(INode node, UniGraph ownerGraph)
        {
            sourceId   = node.Id;
            nodeName = node.ItemName;
            sourceGraph = ownerGraph;
            nodeSubject.Value = node;
            
            position = new Rect(node.Position,new Vector2(node.Width,100));
            
            UpdatePorts();
        }

        #region public methods

        public IEnumerable<PortData> GetPorts(PortIO direction)
        {
            if(SourceNode == null)
                yield break;
            foreach (var port in SourceNode.Ports) {
                if (port.Direction == direction) 
                    yield return GetPortData(port);
            }
        }

        public PortData GetPortData(INodePort port)
        {
            if (portData.TryGetValue(port, out var data))
                return data;
            
            var targetType = port.ValueType;
            targetType ??= typeof(object);
            
            data = new PortData() {
                acceptMultipleEdges = port.ConnectionType == ConnectionType.Multiple,
                displayName         = port.ItemName,
                displayType         = targetType,
                identifier          = port.ItemName,
            };
            
            portData[port] = data;
            return data;
        }

        public override void InitializePorts()
        {
            base.InitializePorts();
            UpdatePorts();
        }

        #endregion
        
        #region custom port definition
        
        [CustomPortBehavior(nameof(inputs))]
        public IEnumerable< PortData > GetPortsForInputs(List< SerializableEdge > edges) => GetPorts(PortIO.Input);
        
        [CustomPortBehavior(nameof(outputs))]
        public IEnumerable< PortData > GetPortsForOutputs(List< SerializableEdge > edges) => GetPorts(PortIO.Output);

        #endregion
        
        #region private methods
        
        protected virtual string GetNodeStyle()
        {
            var nodeStyle = SourceNode == null ? base.layoutStyle : SourceNode.GetStyle();
            nodeStyle = string.IsNullOrEmpty(nodeStyle) ? base.layoutStyle : nodeStyle;
            return nodeStyle;
        }
                
        private void UpdatePorts()
        {
            var sourceNode = SourceNode;
            if (sourceNode == null) return;
            
            foreach (var port in sourceNode.Ports) {
                
                var fieldName = port.IsInput ? nameof(inputs) : nameof(outputs);
                
                var data     = GetPortData(port);
                var basePort = GetPort(fieldName, data.identifier);
                if(basePort!=null)
                    continue;
                
                AddPort(port.IsInput,fieldName,data);
            }
        }
        
        #endregion
    }
}
