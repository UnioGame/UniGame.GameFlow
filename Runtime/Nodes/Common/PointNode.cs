namespace UniModules.UniGame.GameFlow.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Core;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.Attributes;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes;
    
    [Serializable]
    [NodeAsset(typeof(PointNodeData))]
    [CreateNodeMenu("Common/Point")]
    [NodeInfo(nameof(PointNode), "flow", "serializable node for transfer data from input port to output")]
    public class PointNode : SNode
    {
        [Port(PortIO.Input)] private object input;
        [Port(PortIO.Output)] private object output;

        public IPortValue inputPort;
        public IPortValue outputPort;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            inputPort = this.UpdatePortValue(nameof(input), PortIO.Input);
            outputPort = this.UpdatePortValue(nameof(output), PortIO.Output);
            inputPort.Broadcast(outputPort).AddTo(LifeTime);
        }

        protected sealed override UniTask OnExecute()
        {
            return UniTask.CompletedTask;
        }
    }
}