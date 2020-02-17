namespace UniGreenModules.UniGameSystems.Examples.ServiceNode
{
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UniNodeSystem.Runtime.Core;
    using UniRx;

    [CreateNodeMenu("Examples/TypeNodes/SummatorNode")]
    public class SummatorDemoNode : UniNode
    {
        [PortValue(PortIO.Input)]
        public int intValue = 0;

        [PortValue(PortIO.Input)]
        public float floatValue = 0;

        
        [PortValue(PortIO.Output)]
        public int summIntResult = 0;
        
        [PortValue(PortIO.Output)]
        public float summFloatResult = 0f;
        
        protected override void OnExecute()
        {
            var intSumm = GetPort(nameof(intValue));
            var floatSumm = GetPort(nameof(floatValue));
            
            var intOutput   = GetPort(nameof(summIntResult)).Value;
            var floatOutput = GetPort(nameof(summFloatResult)).Value;
            
            intSumm.Value.Receive<int>().
                Do(x => this.summIntResult += x).
                Do(x => intOutput.Publish(summIntResult)).
                Subscribe().
                AddTo(LifeTime);
            
            floatSumm.Value.Receive<float>().
                Do(x => this.summFloatResult += x).
                Do(x => floatOutput.Publish(summFloatResult)).
                Subscribe().
                AddTo(LifeTime);
        }


    }
}
