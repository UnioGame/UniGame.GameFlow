using UniGreenModules.UniCore.Runtime.ProfilerTools;
using UniGreenModules.UniCore.Runtime.Rx.Extensions;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.DemoGame.Runtime.Models;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
using UniRx;

[CreateNodeMenu("Examples/DemoGame/DemoGameWaitGameReady")]
public class DemoGameWaitGameReadyNode : ContextNode
{
    public bool isReady;

    protected override void OnExecute()
    {
        Source.Select(x => x.Receive<DemoGameStatusData>()).
            Switch().
            Select(x => x.IsReady).
            Where(x => x.Value).
            Do(x => GameLog.Log("Game Ready Status")).
            Do(x => Finish()).
            Subscribe().
            AddTo(LifeTime);
    }
}
