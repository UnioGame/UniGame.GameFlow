namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.DemoGame.Runtime.Nodes
{
    using Models;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodes.Nodes.Runtime.Nodes;
    using UniNodeSystem.Runtime.Core;
    using UniRx;

    [CreateNodeMenu("Examples/DemoGame/DemoGameSetGameReady")]
    public class DemoGameSetGameReadyNode : ContextNode
    {
        protected override void OnExecute()
        {
            Source.Select(x => x.Receive<DemoGameStatusData>()).
                Switch().
                Do(x => x.IsReady.Value = true).
                Subscribe().
                AddTo(LifeTime);
        }
    }
}
