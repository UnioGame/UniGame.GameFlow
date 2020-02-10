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
        public int summInt = 0;

        [PortValue(PortIO.Output)]
        public int summResult = 0;
        
        [PortValue(PortIO.Input)]
        public float summFloat = 0f;
        
        protected override void OnExecute()
        {
            var intSumm = GetPort(nameof(summInt));
            var floatSumm = GetPort(nameof(summFloat));
            
            intSumm.Receive<int>().
                Do(x => this.summInt += x).
                Subscribe().
                AddTo(LifeTime);
            
            floatSumm.Receive<float>().
                Do(x => this.summFloat += x).
                Subscribe().
                AddTo(LifeTime);
        }


    }
}
