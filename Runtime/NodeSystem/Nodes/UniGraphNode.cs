﻿using UniModules.GameFlow.Runtime.Attributes;

namespace UniModules.GameFlow.Runtime.Core
{
    using Cysharp.Threading.Tasks;
    using Extensions;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using UniGame.Context.Runtime.Extension;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using global::UniGame.Core.Runtime;
    using UniRx;

    [HideNode]
    public abstract class UniGraphNode : UniNode
    {
        
        public abstract UniGraph LoadOrigin();
        
        protected override void OnInitialize()
        {
            
            base.OnInitialize();

            var sourceGraphPrefab = LoadOrigin();
            
            if (!sourceGraphPrefab) {
                return;
            }
            
            //create node port values by target graph
            foreach (var input in sourceGraphPrefab.Inputs) {
                this.UpdatePortValue(input.ItemName, input.Direction);
            }
            foreach (var output in sourceGraphPrefab.Outputs) {
                this.UpdatePortValue(output.ItemName, output.Direction);
            }
        }

        protected override async UniTask OnExecute()
        {
            var graphPrefab = CreateGraph(LifeTime);
            if (!graphPrefab) return;

            await graphPrefab.ExecuteAsync();

            foreach (var port in Ports) {
                var portName = port.ItemName;
                var originPort = GetPort(portName);
                var targetPort = graphPrefab.GetPort(portName);
                ConnectToGraphPort(port,targetPort, originPort.Direction);
            }

            graphPrefab.AddTo(LifeTime);
        }

        protected abstract UniGraph CreateGraph(ILifeTime lifeTime);
        
        private void ConnectToGraphPort(INodePort sourcePort, INodePort targetPort, PortIO direction)
        {
            var source    = direction == PortIO.Input ? sourcePort : targetPort;
            var target    = direction == PortIO.Input ? targetPort : sourcePort;

            source.Value.
                Broadcast(target.Value).
                AddTo(LifeTime);
        }
        
        
    }
}
