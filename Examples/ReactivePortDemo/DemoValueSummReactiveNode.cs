namespace UniGame.UniNodes.Examples.ReactivePortDemo
{
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.ReactivePorts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;

    [CreateNodeMenu("Examples/ReactivePortDemo/ReactiveSumm","ReactiveSumm")]
    public class DemoValueSummReactiveNode : UniNode
    {
    
        [ReactivePort]
        public IntReactivePort IntIn = new IntReactivePort();

        [ReactivePort(PortIO.Output)]
        public IntReactivePort IntResult = new IntReactivePort();

        protected override void OnInitialize()
        {
            IntIn.Bind(this,nameof(IntIn));
            IntResult.Bind(this,nameof(IntResult));
        }
        
        protected override void OnExecute()
        {
            base.OnExecute();

            IntIn.Do(value => IntResult.Publish(IntResult.Value + value)).
                Subscribe().
                AddTo(LifeTime);
            
        }
    }
}
