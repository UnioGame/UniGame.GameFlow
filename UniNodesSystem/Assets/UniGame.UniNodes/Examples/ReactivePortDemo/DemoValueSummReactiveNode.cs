using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.ReactivePorts;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;

    [CreateNodeMenu("Examples/ReactivePortDemo/ReactiveSumm","ReactiveSumm")]
    public class DemoValueSummReactiveNode : UniNode
    {
    
        [ReactivePort]
        public IntReactivePort IntIn = new IntReactivePort();

        [ReactivePort(PortIO.Output)]
        public IntReactivePort IntResult = new IntReactivePort();

        protected override void OnExecute()
        {
            
            base.OnExecute();

            IntIn.Do(value => IntResult.SetValue(IntResult.Value - value)).
                Subscribe().
                AddTo(LifeTime);
            
        }
    }
}
