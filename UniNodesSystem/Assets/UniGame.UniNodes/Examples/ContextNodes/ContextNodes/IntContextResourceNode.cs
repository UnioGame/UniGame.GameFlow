using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using UniCore.Runtime.Rx.Extensions;
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
