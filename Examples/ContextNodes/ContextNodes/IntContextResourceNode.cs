namespace UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using Nodes.Runtime.Common;
    using NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;

    [CreateNodeMenu("Examples/ContextNodes/IntContext","IntContext")]
    public class IntContextResourceNode : ContextNode
    {
        public int currentIntSummValue;
        public int lastIntValue;
        
        protected override void OnExecute()
        {
            Receive<int>().
                Do(x => lastIntValue = x).
                Do(x => currentIntSummValue += x).
                Subscribe().
                AddTo(LifeTime);
        }
    }
}
