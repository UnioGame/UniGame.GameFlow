using GraphProcessor;

namespace UniGame.GameFlowEditor.Runtime
{
    using System;
    using System.Collections.Generic;
    using UniNodes.NodeSystem.Runtime.Attributes;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class UniBaseNode : BaseNode
    {
        #region inspector
        
        [Input(name = nameof(inputs), allowMultiple = true)]
        public IEnumerable< object > inputs = null;
        
        [Output(name = nameof(outputs), allowMultiple = true)]
        public IEnumerable< object > outputs = null;
        
        #endregion
        
        private Dictionary<INodePort,PortData> portDatas = new Dictionary<INodePort, PortData>(8);
        
        #region public properties
        
        public INode SourceNode { get; protected set; }

        public override string name => SourceNode == null ? base.name : SourceNode.ItemName;

        #endregion
        
        public void Initialize(INode node)
        {
            SourceNode = node;

            position = new Rect(node.Position,new Vector2(node.Width,100));
            
            UpdatePorts();
        }

        #region public methods

        public IEnumerable<PortData> GetPorts(PortIO direction)
        {
            if(SourceNode == null)
                yield break;
            foreach (var port in SourceNode.Ports) {
                if (port.Direction == direction) {
                    yield return GetPortData(port);
                }
            }
        }

        public PortData GetPortData(INodePort port)
        {
            if (portDatas.TryGetValue(port, out var portData))
                return portData;
            var targetType = port.ValueType;
            targetType = targetType == null ? typeof(object) : targetType;
            
            portData = new PortData() {
                acceptMultipleEdges = port.ConnectionType == ConnectionType.Multiple,
                displayName         = port.ItemName,
                displayType         = targetType,
                identifier          = port.ItemName,
            };
            
            portDatas[port] = portData;
            return portData;
        }

        #endregion
        
        #region custom port definition
        
        [CustomPortBehavior(nameof(inputs))]
        public IEnumerable< PortData > GetPortsForInputs(List< SerializableEdge > edges)
        {
            return GetPorts(PortIO.Input);
        }
        
        [CustomPortBehavior(nameof(outputs))]
        public IEnumerable< PortData > GetPortsForOutputs(List< SerializableEdge > edges)
        {
            return GetPorts(PortIO.Output);
        }

        #endregion
        
        
        #region private methods
        
                
        private void UpdatePorts()
        {
            foreach (var port in SourceNode.Ports) {
                var portData = GetPortData(port);
                var fieldName = port.IsInput ? 
                    nameof(inputs) : 
                    nameof(outputs);
                AddPort(port.IsInput,fieldName,portData);
            }
        }
        
        #endregion
    }
}
