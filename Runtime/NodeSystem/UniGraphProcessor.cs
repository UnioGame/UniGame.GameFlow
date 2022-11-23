namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using Runtime.Interfaces;
    using UniCore.Runtime.AsyncOperations;
    using UniCore.Runtime.Rx.Extensions;
    using UniGame.Core.Runtime.AsyncOperations;
    using global::UniGame.Core.Runtime;
    using global::UniGame.Core.Runtime.Extension;
    using UniRx;

    [Serializable]
    public class UniGraphProcessor : AsyncState<IUniGraph, AsyncStatus>, IUniGraphProcessor
    {
        
        protected sealed override async UniTask<AsyncStatus> OnExecute(IUniGraph uniGraph, ILifeTime executionLifeTime)
        {
            var cancellationToken = executionLifeTime.TokenSource;
            
            UpdateCancellationNodes(uniGraph);
                      
            uniGraph.InputsPorts.ForEach(x => BindConnections(uniGraph,uniGraph.GetPort(x.ItemName),x.PortValue) );
            uniGraph.OutputsPorts.ForEach(x => BindConnections(uniGraph,uniGraph.GetPort(x.ItemName),x.PortValue));
            
            var uniNodes = uniGraph.Nodes
                .OfType<IUniNode>()
                .ToList();
            
            foreach (var node in uniNodes)
            {
                uniGraph.AddCleanUpAction(node.Exit);
                node.Ports.ForEach(x => BindConnections(uniGraph, x, x.Value));
            }

            foreach (var uniNode in uniNodes)
            {
                uniNode.ExecuteAsync()
                    .AttachExternalCancellation(uniGraph.LifeTime.TokenSource)
                    .Forget();
            }
            
            await this.AwaitWhileAsync(() => !cancellationToken.IsCancellationRequested,cancellationToken);
            
            return AsyncStatus.Succeeded;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindConnections(IUniNode node,INodePort sourcePort,IContext publisher)
        {
            //data source connections allowed only for input ports
            if (sourcePort.Direction != PortIO.Input) 
                return;
            
            var connections = sourcePort.Connections;
            
            for (var i = 0; i < connections.Count; i++) {
                var connection = connections[i];
                var port       = connection.Port;
                if(port == null || port.Direction == PortIO.Input || port.NodeId == node.Id)
                    continue;
                
                port.Broadcast(publisher).AddTo(LifeTime);
            }
            
        }


        private void UpdateCancellationNodes(IUniGraph uniGraph)
        {
            foreach (var node in uniGraph.Nodes)
            {
                if (!(node is IGraphCancelationNode cancelation)) continue;
                
                cancelation.PortValue.PortValueChanged.
                    Subscribe(unit => uniGraph.Exit()).
                    AddTo(LifeTime);
            }

        }

    }
}