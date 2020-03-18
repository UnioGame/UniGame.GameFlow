namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using Extensions;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;

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

        protected override void OnExecute()
        {
            base.OnExecute();
            
            var graphPrefab = CreateGraph(LifeTime);
            if (!graphPrefab) {
                return;
            }

            graphPrefab.Execute();

            foreach (var port in Ports) {
                var portName = port.ItemName;
                var originPort = GetPort(portName);
                var targetPort = graphPrefab.GetPort(portName);
                ConnectToGraphPort(port,targetPort, originPort.Direction);
            }
            
            LifeTime.AddCleanUpAction(() => graphPrefab?.Exit());
        }

        protected abstract UniGraph CreateGraph(ILifeTime lifeTime);
        
        private void ConnectToGraphPort(INodePort sourcePort, INodePort targetPort, PortIO direction)
        {
            var source    = direction == PortIO.Input ? sourcePort : targetPort;
            var target    = direction == PortIO.Input ? targetPort : sourcePort;

            source.Value.
                Bind(target.Value).
                AddTo(LifeTime);
        }
        
        
    }
}
