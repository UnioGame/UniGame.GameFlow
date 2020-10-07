namespace UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using Nodes.Runtime.Common;
    using NodeSystem.Runtime.Core;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;

    [CreateNodeMenu("Examples/ContextNodes/FloatContex","FloatContex")]
    public class FloatContexResourceNode : ContextNode
    {
        public float currentIntSummValue;
        public float lastIntValue;
        
        protected override void OnExecute()
        {
            Receive<float>().
                Do(x => GameLog.Log($"{ItemName} : VALUE {x}")).
                Do(x => lastIntValue        =  x).
                Do(x => currentIntSummValue += x).
                Subscribe().
                AddTo(LifeTime);
        }
    }
}
