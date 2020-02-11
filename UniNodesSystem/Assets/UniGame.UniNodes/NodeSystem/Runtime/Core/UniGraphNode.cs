namespace UniGreenModules.UniNodeSystem.Nodes
{
    using Runtime.Core;
    using Runtime.Extensions;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;

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
                this.UpdatePortValue(input.FieldName, input.Direction);
            }
            foreach (var output in sourceGraphPrefab.Outputs) {
                this.UpdatePortValue(output.FieldName, output.Direction);
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

            source.Bind(target).
                AddTo(LifeTime);
        }
        
        
    }
}
