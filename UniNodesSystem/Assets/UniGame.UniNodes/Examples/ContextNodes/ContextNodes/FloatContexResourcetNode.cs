using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Runtime.Core;
    using UniRx;

    [CreateNodeMenu("Examples/ContextNodes/FloatContex","FloatContex")]
    public class FloatContexResourcetNode : ContextNode
    {
        public float currentIntSummValue;
        public float lastIntValue;
        
        protected override void OnExecute()
        {
            Receive<float>().
                Do(x => lastIntValue        =  x).
                Do(x => currentIntSummValue += x).
                Subscribe().
                AddTo(LifeTime);
        }
    }
}
