using Cysharp.Threading.Tasks;
using GraphProcessor;
using UniModules.GameFlow.Runtime.Core;
using UniModules.GameFlow.Runtime.Extensions;
using UniModules.GameFlow.Runtime.Interfaces;
using NodePort = GraphProcessor.NodePort;

namespace UniGame.GameFlow
{
    using System;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core.Nodes;

    [Serializable]
    [HideNode]
    public class GraphProcessorNode : SNode,IRuntimeOnlyNode
    {
        private readonly UniGraph graph;
        private readonly BaseNode node;

        public override string ItemName => node.name;

        public GraphProcessorNode(UniGraph graph,BaseNode node)
        {
            this.graph = graph;
            this.node = node;
        }

        protected sealed override void OnInitialize()
        {
            base.OnInitialize();

            foreach (var port in node.inputPorts)
                UpdatePort(port, PortIO.Input);
            
            foreach (var port in node.outputPorts)
                UpdatePort(port, PortIO.Output);
        }

        protected sealed override UniTask OnExecute()
        {
            return base.OnExecute();
        }

        private void UpdatePort(NodePort port, PortIO direction)
        {
            var portData = port.portData;
            var connectionType = portData.acceptMultipleEdges ? ConnectionType.Multiple : ConnectionType.Override;
            var portValue = this.UpdatePortValue(port.fieldName, direction,connectionType,ShowBackingValue.Always);
            var nodePort = GetPort(port.fieldName);

            
        }
    }

    public interface IRuntimeOnlyNode
    {
        
    }
}
